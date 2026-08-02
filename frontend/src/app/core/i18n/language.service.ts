import { Injectable, computed, effect, signal } from '@angular/core';
import { Category, Product } from '../models/shop.models';
import { Lang, TRANSLATIONS, TranslationKey } from './translations';

const STORAGE_KEY = 'velora-lang';

@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly langSignal = signal<Lang>(this.readInitial());

  readonly lang = this.langSignal.asReadonly();
  readonly isRtl = computed(() => this.langSignal() === 'fa');
  readonly locale = computed(() => (this.langSignal() === 'fa' ? 'fa' : 'en-US'));

  constructor() {
    effect(() => {
      const lang = this.langSignal();
      document.documentElement.lang = lang;
      document.documentElement.dir = lang === 'fa' ? 'rtl' : 'ltr';
      document.body.classList.toggle('lang-fa', lang === 'fa');
      document.body.classList.toggle('lang-en', lang === 'en');
      localStorage.setItem(STORAGE_KEY, lang);
    });
  }

  setLang(lang: Lang): void {
    this.langSignal.set(lang);
  }

  toggle(): void {
    this.setLang(this.langSignal() === 'en' ? 'fa' : 'en');
  }

  t(key: TranslationKey, params?: Record<string, string | number>): string {
    let value = TRANSLATIONS[this.langSignal()][key] ?? TRANSLATIONS.en[key] ?? key;
    if (params) {
      for (const [name, paramValue] of Object.entries(params)) {
        value = value.replaceAll(`{${name}}`, String(paramValue));
      }
    }
    return value;
  }

  productName(product: Pick<Product, 'name' | 'nameFa'>): string {
    return this.langSignal() === 'fa' ? product.nameFa || product.name : product.name;
  }

  productShort(product: Pick<Product, 'shortDescription' | 'shortDescriptionFa'>): string {
    return this.langSignal() === 'fa'
      ? product.shortDescriptionFa || product.shortDescription
      : product.shortDescription;
  }

  productDescription(
    product: Pick<Product, 'description' | 'descriptionFa' | 'shortDescription' | 'shortDescriptionFa'>
  ): string {
    if (this.langSignal() === 'fa') {
      return product.descriptionFa || product.description || product.shortDescriptionFa || product.shortDescription;
    }
    return product.description || product.shortDescription;
  }

  categoryName(category: Pick<Category, 'name' | 'nameFa'> | Pick<Product, 'categoryName' | 'categoryNameFa'>): string {
    if ('categoryName' in category) {
      return this.langSignal() === 'fa'
        ? category.categoryNameFa || category.categoryName
        : category.categoryName;
    }
    return this.langSignal() === 'fa' ? category.nameFa || category.name : category.name;
  }

  skinType(value: string): string {
    const key = `skin.${value}` as TranslationKey;
    return TRANSLATIONS[this.langSignal()][key] ?? value;
  }

  private readInitial(): Lang {
    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (saved === 'fa' || saved === 'en') {
        return saved;
      }
    } catch {
      // ignore
    }
    return 'en';
  }
}
