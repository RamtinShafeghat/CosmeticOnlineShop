import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AdminOrderListItem, Category, Product } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, CurrencyPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly api = inject(ApiService);

  readonly categories = signal<Category[]>([]);
  readonly products = signal<Product[]>([]);
  readonly orders = signal<AdminOrderListItem[]>([]);
  readonly error = signal<string | null>(null);

  readonly featuredCount = computed(
    () => this.products().filter((product) => product.isFeatured).length
  );
  readonly lowStockCount = computed(
    () => this.products().filter((product) => product.stock < 20).length
  );
  readonly revenue = computed(() =>
    this.orders().reduce((sum, order) => sum + order.total, 0)
  );

  ngOnInit(): void {
    this.api.getCategories().subscribe({
      next: (items) => this.categories.set(items),
      error: () => this.error.set('Unable to load dashboard data.')
    });
    this.api.getProducts().subscribe({
      next: (items) => this.products.set(items),
      error: () => this.error.set('Unable to load dashboard data.')
    });
    this.api.getOrders().subscribe({
      next: (items) => this.orders.set(items),
      error: () => this.error.set('Unable to load dashboard data.')
    });
  }
}
