import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Component, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Product } from '../../core/models/shop.models';
import { AuthService } from '../../core/services/auth.service';
import { WishlistService } from '../../core/services/wishlist.service';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DecimalPipe, AssetUrlPipe, TranslatePipe],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input({ required: true }) product!: Product;
  readonly i18n = inject(LanguageService);
  readonly wishlist = inject(WishlistService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  readonly wishlistBusy = signal(false);

  toggleWishlist(event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    if (!this.auth.isAuthenticated()) {
      void this.router.navigate(['/login']);
      return;
    }

    if (this.wishlistBusy()) {
      return;
    }

    this.wishlistBusy.set(true);
    this.wishlist.toggle(this.product).subscribe({
      next: () => this.wishlistBusy.set(false),
      error: () => this.wishlistBusy.set(false)
    });
  }
}
