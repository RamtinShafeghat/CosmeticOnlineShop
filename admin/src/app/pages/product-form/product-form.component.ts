import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { of, switchMap } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Category, Product, UpsertProduct } from '../../core/models';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [FormsModule, RouterLink, AssetUrlPipe, TranslatePipe],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})
export class ProductFormComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly i18n = inject(LanguageService);

  id: number | null = null;
  categories: Category[] = [];
  selectedFile: File | null = null;
  previewUrl: string | null = null;
  /** Stock captured when the edit form loaded; used to detect intentional stock edits. */
  private loadedStock: number | null = null;

  form: UpsertProduct = {
    name: '',
    nameFa: '',
    slug: '',
    description: '',
    descriptionFa: '',
    shortDescription: '',
    shortDescriptionFa: '',
    price: 0,
    imageUrl: '',
    brand: 'Velora',
    skinType: 'All',
    stock: 0,
    isFeatured: false,
    categoryId: 0
  };

  readonly saving = signal(false);
  readonly uploading = signal(false);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  readonly skinTypes = ['All', 'Dry', 'Normal', 'Sensitive'] as const;

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        if (!this.form.categoryId && categories.length > 0) {
          this.form.categoryId = categories[0].id;
        }
      },
      error: () => this.error.set(this.i18n.t('productForm.loadCategoriesFailed'))
    });

    const rawId = this.route.snapshot.paramMap.get('id');
    if (rawId && rawId !== 'new') {
      this.id = Number(rawId);
      this.api.getProduct(this.id).subscribe({
        next: (product) => {
          this.loadedStock = product.stock;
          this.form = {
            name: product.name,
            nameFa: product.nameFa,
            slug: product.slug,
            description: product.description || '',
            descriptionFa: product.descriptionFa || '',
            shortDescription: product.shortDescription,
            shortDescriptionFa: product.shortDescriptionFa,
            price: product.price,
            imageUrl: product.imageUrl,
            brand: product.brand,
            skinType: product.skinType,
            stock: product.stock,
            isFeatured: product.isFeatured,
            categoryId: product.categoryId
          };
          this.previewUrl = product.imageUrl || null;
        },
        error: () => this.error.set(this.i18n.t('productForm.notFound'))
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
    if (!this.form.categoryId) {
      this.error.set(this.i18n.t('productForm.chooseCategory'));
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    this.message.set(null);

    const payload: UpsertProduct = {
      ...this.form,
      slug: this.form.slug || undefined,
      imageUrl: this.form.imageUrl || undefined
    };

    const stockChanged =
      this.id != null && this.loadedStock != null && this.form.stock !== this.loadedStock;

    const request$ =
      this.id == null
        ? this.api.createProduct(payload)
        : this.api.updateProduct(this.id, payload).pipe(
            switchMap((product) =>
              stockChanged && this.loadedStock != null
                ? this.api.updateProductStock(
                    product.id,
                    this.form.stock,
                    this.loadedStock
                  )
                : of(product)
            )
          );

    request$.subscribe({
      next: (product: Product) => {
        this.id = product.id;
        this.form.imageUrl = product.imageUrl;
        this.loadedStock = product.stock;
        this.form.stock = product.stock;
        if (this.selectedFile) {
          this.uploadImage(product.id);
        } else {
          this.saving.set(false);
          void this.router.navigate(['/products']);
        }
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(err?.error?.message || this.i18n.t('productForm.saveFailed'));
      }
    });
  }

  private uploadImage(productId: number): void {
    if (!this.selectedFile) {
      return;
    }
    this.uploading.set(true);
    this.api.uploadProductImage(productId, this.selectedFile).subscribe({
      next: (result) => {
        this.form.imageUrl = result.imageUrl;
        this.previewUrl = result.imageUrl;
        this.selectedFile = null;
        this.uploading.set(false);
        this.saving.set(false);
        void this.router.navigate(['/products']);
      },
      error: (err) => {
        this.uploading.set(false);
        this.saving.set(false);
        this.error.set(err?.error?.message || this.i18n.t('productForm.uploadFailed'));
      }
    });
  }
}
