import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { Category } from '../../core/models';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [RouterLink, TranslatePipe],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  private readonly api = inject(ApiService);
  readonly i18n = inject(LanguageService);

  readonly categories = signal<Category[]>([]);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getCategories().subscribe({
      next: (items) => this.categories.set(items),
      error: () => this.error.set(this.i18n.t('categories.loadError'))
    });
  }

  remove(category: Category): void {
    if (!confirm(this.i18n.t('categories.deleteConfirm', { name: category.name }))) {
      return;
    }
    this.api.deleteCategory(category.id).subscribe({
      next: () => {
        this.message.set(this.i18n.t('categories.deleted', { name: category.name }));
        this.reload();
      },
      error: (err) => {
        this.error.set(err?.error?.message || this.i18n.t('categories.deleteFailed'));
      }
    });
  }
}
