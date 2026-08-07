import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Category, Product, ProductFilterOptions } from '../../core/models/shop.models';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [RouterLink, FormsModule, ProductCardComponent, TranslatePipe],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);
  readonly i18n = inject(LanguageService);

  readonly categories = signal<Category[]>([]);
  readonly products = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly errorKey = signal<'shop.error' | 'shop.categoryMissing' | null>(null);
  readonly activeCategorySlug = signal<string | null>(null);
  readonly brands = signal<string[]>([]);
  readonly skinTypes = signal<string[]>([]);
  search = '';
  brand = '';
  skinType = '';

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (categories) => this.categories.set(categories),
      error: () => this.errorKey.set('shop.error')
    });

    this.productService.getFilterOptions().subscribe({
      next: (options: ProductFilterOptions) => {
        this.brands.set(options.brands);
        // "All" is covered by the default "any skin type" choice.
        this.skinTypes.set(options.skinTypes.filter((s) => s !== 'All'));
      },
      error: () => {
        // Filter dropdowns stay hidden if options cannot load; the shop still works.
      }
    });

    this.route.paramMap.subscribe((params) => {
      const slug = params.get('categorySlug');
      this.activeCategorySlug.set(slug);
      this.loadProducts();
    });
  }

  onSearch(): void {
    this.loadProducts();
  }

  onFilterChange(): void {
    this.loadProducts();
  }

  private loadProducts(): void {
    this.loading.set(true);
    this.errorKey.set(null);

    const slug = this.activeCategorySlug();

    const fetch = () => {
      const categoryId = slug
        ? this.categories().find((c) => c.slug === slug)?.id
        : undefined;

      if (slug && categoryId == null && this.categories().length > 0) {
        this.errorKey.set('shop.categoryMissing');
        this.products.set([]);
        this.loading.set(false);
        return;
      }

      this.productService
        .getProducts({
          categoryId,
          search: this.search.trim() || undefined,
          brand: this.brand || undefined,
          skinType: this.skinType || undefined
        })
        .subscribe({
          next: (products) => {
            this.products.set(products);
            this.loading.set(false);
          },
          error: () => {
            this.errorKey.set('shop.error');
            this.loading.set(false);
          }
        });
    };

    if (slug && this.categories().length === 0) {
      this.categoryService.getCategories().subscribe({
        next: (categories) => {
          this.categories.set(categories);
          fetch();
        },
        error: () => {
          this.errorKey.set('shop.error');
          this.loading.set(false);
        }
      });
      return;
    }

    fetch();
  }
}
