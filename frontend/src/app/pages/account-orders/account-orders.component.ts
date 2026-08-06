import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TranslationKey } from '../../core/i18n/translations';
import { CustomerOrderListItem } from '../../core/models/shop.models';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-account-orders',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './account-orders.component.html',
  styleUrl: './account-orders.component.scss'
})
export class AccountOrdersComponent implements OnInit {
  private readonly account = inject(AccountService);
  readonly i18n = inject(LanguageService);

  readonly orders = signal<CustomerOrderListItem[]>([]);
  readonly loading = signal(true);
  readonly error = signal(false);

  ngOnInit(): void {
    this.account.getOrders().subscribe({
      next: (orders) => {
        this.orders.set(orders);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(true);
        this.loading.set(false);
      }
    });
  }

  statusLabel(status: string): string {
    return this.i18n.t(`order.status.${status}` as TranslationKey);
  }
}
