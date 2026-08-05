import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { AdminOrderListItem } from '../../core/models';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {
  private readonly api = inject(ApiService);

  readonly orders = signal<AdminOrderListItem[]>([]);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.api.getOrders().subscribe({
      next: (items) => this.orders.set(items),
      error: () => this.error.set('Unable to load orders.')
    });
  }
}
