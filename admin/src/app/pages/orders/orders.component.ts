import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { AdminOrderListItem } from '../../core/models';

export type OrderTab = 'Pending' | 'Confirmed';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.scss'
})
export class OrdersComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly i18n = inject(LanguageService);

  readonly tab = signal<OrderTab>('Pending');
  readonly orders = signal<AdminOrderListItem[]>([]);
  readonly error = signal<string | null>(null);
  readonly confirmingId = signal<number | null>(null);

  readonly emptyKey = computed(() =>
    this.tab() === 'Pending' ? 'orders.emptyPending' : 'orders.emptyConfirmed'
  );

  ngOnInit(): void {
    this.load();
  }

  setTab(tab: OrderTab): void {
    if (this.tab() === tab) {
      return;
    }
    this.tab.set(tab);
    this.load();
  }

  load(): void {
    this.error.set(null);
    this.api.getOrders(this.tab()).subscribe({
      next: (items) => this.orders.set(items),
      error: () => this.error.set(this.i18n.t('orders.loadError'))
    });
  }

  confirm(order: AdminOrderListItem, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.confirmingId.set(order.id);
    this.api.confirmOrder(order.id).subscribe({
      next: () => {
        this.confirmingId.set(null);
        this.load();
      },
      error: () => {
        this.confirmingId.set(null);
        this.error.set(this.i18n.t('orderDetail.confirmFailed'));
      }
    });
  }
}
