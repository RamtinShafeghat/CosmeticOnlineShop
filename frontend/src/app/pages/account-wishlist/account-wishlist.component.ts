import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { WishlistService } from '../../core/services/wishlist.service';
import { ProductCardComponent } from '../../shared/product-card/product-card.component';

@Component({
  selector: 'app-account-wishlist',
  standalone: true,
  imports: [RouterLink, ProductCardComponent, TranslatePipe],
  templateUrl: './account-wishlist.component.html',
  styleUrl: './account-wishlist.component.scss'
})
export class AccountWishlistComponent {
  readonly wishlist = inject(WishlistService);
}
