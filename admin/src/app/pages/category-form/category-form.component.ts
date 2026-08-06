import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { LanguageService } from '../../core/i18n/language.service';
import { TranslatePipe } from '../../core/i18n/translate.pipe';
import { UpsertCategory } from '../../core/models';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [FormsModule, RouterLink, TranslatePipe],
  templateUrl: './category-form.component.html',
  styleUrl: './category-form.component.scss'
})
export class CategoryFormComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly i18n = inject(LanguageService);

  id: number | null = null;
  form: UpsertCategory = {
    name: '',
    nameFa: '',
    slug: '',
    description: '',
    descriptionFa: ''
  };

  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const rawId = this.route.snapshot.paramMap.get('id');
    if (rawId && rawId !== 'new') {
      this.id = Number(rawId);
      this.api.getCategory(this.id).subscribe({
        next: (category) => {
          this.form = {
            name: category.name,
            nameFa: category.nameFa,
            slug: category.slug,
            description: category.description,
            descriptionFa: category.descriptionFa
          };
        },
        error: () => this.error.set(this.i18n.t('categoryForm.notFound'))
      });
    }
  }

  save(): void {
    this.saving.set(true);
    this.error.set(null);
    const payload = { ...this.form, slug: this.form.slug || undefined };
    const request$ =
      this.id == null
        ? this.api.createCategory(payload)
        : this.api.updateCategory(this.id, payload);

    request$.subscribe({
      next: () => {
        this.saving.set(false);
        void this.router.navigate(['/categories']);
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(err?.error?.message || this.i18n.t('categoryForm.saveFailed'));
      }
    });
  }
}
