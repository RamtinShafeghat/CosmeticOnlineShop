import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AssetUrlPipe } from '../../core/asset-url.pipe';
import { Product } from '../../core/models';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, AssetUrlPipe],
  templateUrl: './products.component.html',
  styleUrl: './products.component.scss'
})
export class ProductsComponent implements OnInit {
  private readonly api = inject(ApiService);

  readonly products = signal<Product[]>([]);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getProducts().subscribe({
      next: (items) => this.products.set(items),
      error: () => this.error.set('Unable to load products.')
    });
  }

  remove(product: Product): void {
    if (!confirm(`Delete product "${product.name}"?`)) {
      return;
    }
    this.api.deleteProduct(product.id).subscribe({
      next: () => {
        this.message.set(`Deleted ${product.name}.`);
        this.reload();
      },
      error: (err) => {
        this.error.set(err?.error?.message || 'Delete failed.');
      }
    });
  }
}
