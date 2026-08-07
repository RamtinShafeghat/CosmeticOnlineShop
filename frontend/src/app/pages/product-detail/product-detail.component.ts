import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Product } from '../../core/models/shop.models';
import { AuthService } from '../../core/services/auth.service';
import { CartService } from '../../core/services/cart.service';
import { ProductService } from '../../core/services/product.service';
import { WishlistService } from '../../core/services/wishlist.service';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CurrencyPipe, DecimalPipe, FormsModule, RouterLink, TranslatePipe, AssetUrlPipe],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly cart = inject(CartService);
  readonly auth = inject(AuthService);
  readonly i18n = inject(LanguageService);
  readonly wishlist = inject(WishlistService);
  readonly wishlistBusy = signal(false);

  readonly product = signal<Product | null>(null);
  readonly loading = signal(true);
  readonly error = signal(false);
  readonly added = signal(false);
  readonly ratingBusy = signal(false);
  readonly ratingMessage = signal<'saved' | 'failed' | null>(null);
  readonly hoverStars = signal(0);
  quantity = 1;
  readonly starValues = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const slug = params.get('slug');
      if (!slug) {
        this.error.set(true);
        this.loading.set(false);
        return;
      }

      this.loading.set(true);
      this.error.set(false);
      this.added.set(false);
      this.ratingMessage.set(null);
      this.hoverStars.set(0);
      this.product.set(null);
      this.productService.getBySlug(slug).subscribe({
        next: (product) => {
          this.product.set(product);
          this.quantity = 1;
          this.loading.set(false);
        },
        error: () => {
          this.error.set(true);
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

  toggleWishlist(): void {
    const product = this.product();
    if (!product || this.wishlistBusy()) {
      return;
    }

    if (!this.auth.isAuthenticated()) {
      void this.router.navigate(['/login']);
      return;
    }

    this.wishlistBusy.set(true);
    this.wishlist.toggle(product).subscribe({
      next: () => this.wishlistBusy.set(false),
      error: () => this.wishlistBusy.set(false)
    });
  }

  displayStars(): number {
    const product = this.product();
    if (!product) {
      return 0;
    }
    if (this.hoverStars() > 0) {
      return this.hoverStars();
    }
    return product.myRating ?? Math.round(product.averageRating ?? 0);
  }

  setHover(stars: number): void {
    if (!this.auth.isAuthenticated() || this.ratingBusy()) {
      return;
    }
    this.hoverStars.set(stars);
  }

  clearHover(): void {
    this.hoverStars.set(0);
  }

  rate(stars: number): void {
    const product = this.product();
    if (!product || !this.auth.isAuthenticated() || this.ratingBusy()) {
      return;
    }

    this.ratingBusy.set(true);
    this.ratingMessage.set(null);
    this.productService.rateProduct(product.id, stars).subscribe({
      next: (summary) => {
        this.product.set({
          ...product,
          averageRating: summary.averageRating,
          ratingCount: summary.ratingCount,
          myRating: summary.myRating
        });
        this.ratingBusy.set(false);
        this.ratingMessage.set('saved');
        this.hoverStars.set(0);
      },
      error: () => {
        this.ratingBusy.set(false);
        this.ratingMessage.set('failed');
      }
    });
  }
}
