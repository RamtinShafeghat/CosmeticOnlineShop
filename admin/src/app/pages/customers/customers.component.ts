import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ApiService } from '../../core/api.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { AdminCustomerListItem } from '../../core/models';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [RouterLink, CurrencyPipe, DatePipe, FormsModule, TranslatePipe],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.scss'
})
export class CustomersComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly i18n = inject(LanguageService);

  readonly customers = signal<AdminCustomerListItem[]>([]);
  readonly error = signal<string | null>(null);

  search = '';
  private readonly search$ = new Subject<string>();

  constructor() {
    this.search$
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(() => this.load());
  }

  ngOnInit(): void {
    this.load();
  }

  onSearchChange(value: string): void {
    this.search = value;
    this.search$.next(value.trim());
  }

  load(): void {
    this.error.set(null);
    this.api.getCustomers(this.search.trim() || undefined).subscribe({
      next: (items) => this.customers.set(items),
      error: () => this.error.set(this.i18n.t('customers.loadError'))
    });
  }
}
