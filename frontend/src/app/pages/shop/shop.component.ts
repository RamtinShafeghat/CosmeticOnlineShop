import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Category, Product } from '../../core/models/shop.models';
import { CategoryService } from '../../core/services/category.service';
import { ProductService } from '../../core/services/product.service';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [RouterLink, FormsModule, ProductCardComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly route = inject(ActivatedRoute);

  readonly categories = signal<Category[]>([]);
  readonly products = signal<Product[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly activeCategorySlug = signal<string | null>(null);
  search = '';

  ngOnInit(): void {
    this.categoryService.getCategories().subscribe({
      next: (categories) => this.categories.set(categories),
      error: () => this.error.set('Unable to load categories.')
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

  private loadProducts(): void {
    this.loading.set(true);
    this.error.set(null);

    const slug = this.activeCategorySlug();
    const category = this.categories().find((c) => c.slug === slug);

    const fetch = () => {
      const categoryId = slug
        ? this.categories().find((c) => c.slug === slug)?.id
        : undefined;

      if (slug && categoryId == null && this.categories().length > 0) {
        this.error.set('Category not found.');
        this.products.set([]);
        this.loading.set(false);
        return;
      }

      this.productService
        .getProducts({
          categoryId,
          search: this.search.trim() || undefined
        })
        .subscribe({
          next: (products) => {
            this.products.set(products);
            this.loading.set(false);
          },
          error: () => {
            this.error.set('Unable to load products. Is the API running?');
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
          this.error.set('Unable to load categories.');
          this.loading.set(false);
        }
      });
      return;
    }

    void category;
    fetch();
  }
}
