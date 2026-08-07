import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Product, UpsertCarouselSlide } from '../../core/models';

@Component({
  selector: 'app-carousel-form',
  standalone: true,
  imports: [FormsModule, RouterLink, AssetUrlPipe, TranslatePipe],
  templateUrl: './carousel-form.component.html',
  styleUrl: './carousel-form.component.scss'
})
export class CarouselFormComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly i18n = inject(LanguageService);

  id: number | null = null;
  products: Product[] = [];
  selectedFile: File | null = null;
  previewUrl: string | null = null;

  form: UpsertCarouselSlide = {
    title: '',
    titleFa: '',
    imageUrl: '',
    linkUrl: '',
    productId: null,
    sortOrder: 0,
    isActive: true
  };

  readonly saving = signal(false);
  readonly uploading = signal(false);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getProducts().subscribe({
      next: (products) => (this.products = products),
      error: () => this.error.set(this.i18n.t('carouselForm.loadProductsFailed'))
    });

    const rawId = this.route.snapshot.paramMap.get('id');
    if (rawId && rawId !== 'new') {
      this.id = Number(rawId);
      this.api.getCarouselSlide(this.id).subscribe({
        next: (slide) => {
          this.form = {
            title: slide.title,
            titleFa: slide.titleFa,
            imageUrl: slide.imageUrl,
            linkUrl: slide.linkUrl,
            productId: slide.productId,
            sortOrder: slide.sortOrder,
            isActive: slide.isActive
          };
          this.previewUrl = slide.imageUrl || null;
        },
        error: () => this.error.set(this.i18n.t('carouselForm.notFound'))
      });
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.selectedFile = file;
    if (file) {
      this.previewUrl = URL.createObjectURL(file);
    }
  }

  save(): void {
    this.saving.set(true);
    this.error.set(null);
    this.message.set(null);

    const payload: UpsertCarouselSlide = {
      ...this.form,
      imageUrl: this.form.imageUrl || undefined,
      linkUrl: this.form.linkUrl || undefined,
      productId: this.form.productId || null
    };

    const request$ =
      this.id == null
        ? this.api.createCarouselSlide(payload)
        : this.api.updateCarouselSlide(this.id, payload);

    request$.subscribe({
      next: (slide) => {
        this.id = slide.id;
        this.form.imageUrl = slide.imageUrl;
        if (this.selectedFile) {
          this.uploadImage(slide.id);
        } else {
          this.saving.set(false);
          void this.router.navigate(['/carousel']);
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(err?.error?.message || this.i18n.t('carouselForm.saveFailed'));
      }
    });
  }

  private uploadImage(slideId: number): void {
    if (!this.selectedFile) {
      return;
    }
    this.uploading.set(true);
    this.api.uploadCarouselSlideImage(slideId, this.selectedFile).subscribe({
      next: (result) => {
        this.form.imageUrl = result.imageUrl;
        this.previewUrl = result.imageUrl;
        this.selectedFile = null;
        this.uploading.set(false);
        this.saving.set(false);
        void this.router.navigate(['/carousel']);
      },
      error: (err) => {
        this.uploading.set(false);
        this.saving.set(false);
        this.error.set(err?.error?.message || this.i18n.t('carouselForm.uploadFailed'));
      }
    });
  }
}
