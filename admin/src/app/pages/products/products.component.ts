import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Product } from '../../core/models';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, AssetUrlPipe, TranslatePipe],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly i18n = inject(LanguageService);

  readonly products = signal<Product[]>([]);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getProducts().subscribe({
      next: (items) => this.products.set(items),
      error: () => this.error.set(this.i18n.t('products.loadError'))
    });
  }

  remove(product: Product): void {
    if (!confirm(this.i18n.t('products.deleteConfirm', { name: product.name }))) {
      return;
    }
    this.api.deleteProduct(product.id).subscribe({
      next: () => {
        this.message.set(this.i18n.t('products.deleted', { name: product.name }));
        this.reload();
      },
      error: (err) => {
        this.error.set(err?.error?.message || this.i18n.t('products.deleteFailed'));
      }
    });
  }
}
