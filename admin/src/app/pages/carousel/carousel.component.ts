import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { CarouselSlide } from '../../core/models';

@Component({
  selector: 'app-carousel',
  standalone: true,
  imports: [RouterLink, AssetUrlPipe, TranslatePipe],
  templateUrl: './carousel.component.html',
  styleUrl: './carousel.component.scss'
})
export class CarouselComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly i18n = inject(LanguageService);

  readonly slides = signal<CarouselSlide[]>([]);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getCarouselSlides().subscribe({
      next: (items) => this.slides.set(items),
      error: () => this.error.set(this.i18n.t('carousel.loadError'))
    });
  }

  remove(slide: CarouselSlide): void {
    const label = slide.title || slide.titleFa || `#${slide.id}`;
    if (!confirm(this.i18n.t('carousel.deleteConfirm', { name: label }))) {
      return;
    }
    this.api.deleteCarouselSlide(slide.id).subscribe({
      next: () => {
        this.message.set(this.i18n.t('carousel.deleted', { name: label }));
        this.reload();
      },
      error: (err) => {
        this.error.set(err?.error?.message || this.i18n.t('carousel.deleteFailed'));
      }
    });
  }
}
