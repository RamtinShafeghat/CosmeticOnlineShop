import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { TranslationKey } from '../../core/i18n/translations';
import { Order } from '../../core/models/shop.models';
import { AccountService } from '../../core/services/account.service';

@Component({
  selector: 'app-account-order-detail',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, TranslatePipe],
  templateUrl: './account-order-detail.component.html',
  styleUrl: './account-order-detail.component.scss'
})
export class AccountOrderDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly account = inject(AccountService);
  readonly i18n = inject(LanguageService);

  readonly order = signal<Order | null>(null);
  readonly loading = signal(true);
  readonly error = signal(false);

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error.set(true);
      this.loading.set(false);
      return;
    }

    this.account.getOrder(id).subscribe({
      next: (order) => {
        this.order.set(order);
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
