import { CurrencyPipe, DecimalPipe } from '@angular/common';
import { Component, Input, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { Product } from '../../core/models/shop.models';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DecimalPipe, AssetUrlPipe],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input({ required: true }) product!: Product;
  readonly i18n = inject(LanguageService);
}
