import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Product } from '../../core/models/shop.models';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CurrencyPipe, FormsModule, RouterLink],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);
  private readonly cart = inject(CartService);

  readonly product = signal<Product | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly added = signal(false);
  quantity = 1;

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug');
      if (!slug) {
        this.error.set('Product not found.');
        this.loading.set(false);
        return;
      }

      this.loading.set(true);
      this.added.set(false);
      this.productService.getBySlug(slug).subscribe({
        next: (product) => {
          this.product.set(product);
          this.quantity = 1;
          this.loading.set(false);
        },
        error: () => {
          this.error.set('Product not found.');
          this.loading.set(false);
        }
      });
    });
  }

  addToBag(): void {
    const product = this.product();
    if (!product) {
      return;
    }
    this.cart.add(product, this.quantity);
    this.added.set(true);
  }
}
