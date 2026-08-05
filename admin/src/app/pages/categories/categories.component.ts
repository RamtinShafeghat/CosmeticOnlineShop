import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { Category } from '../../core/models';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss'
})
export class CategoriesComponent implements OnInit {
  private readonly api = inject(ApiService);

  readonly categories = signal<Category[]>([]);
  readonly error = signal<string | null>(null);
  readonly message = signal<string | null>(null);

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getCategories().subscribe({
      next: (items) => this.categories.set(items),
      error: () => this.error.set('Unable to load categories.')
    });
  }

  remove(category: Category): void {
    if (!confirm(`Delete category "${category.name}"?`)) {
      return;
    }
    this.api.deleteCategory(category.id).subscribe({
      next: () => {
        this.message.set(`Deleted ${category.name}.`);
        this.reload();
      },
      error: (err) => {
        this.error.set(err?.error?.message || 'Delete failed.');
      }
    });
  }
}
