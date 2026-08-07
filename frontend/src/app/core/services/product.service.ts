import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product, ProductFilterOptions, ProductRatingSummary } from '../models/shop.models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/products`;

  getProducts(options?: {
    categoryId?: number;
    search?: string;
    featured?: boolean;
    brand?: string;
    skinType?: string;
  }): Observable<Product[]> {
    let params = new HttpParams();
    if (options?.categoryId != null) {
      params = params.set('categoryId', options.categoryId);
    }
    if (options?.search) {
      params = params.set('search', options.search);
    }
    if (options?.featured != null) {
      params = params.set('featured', options.featured);
    }
    if (options?.brand) {
      params = params.set('brand', options.brand);
    }
    if (options?.skinType) {
      params = params.set('skinType', options.skinType);
    }
    return this.http.get<Product[]>(this.baseUrl, { params });
  }

  getFilterOptions(): Observable<ProductFilterOptions> {
    return this.http.get<ProductFilterOptions>(`${this.baseUrl}/filters`);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  getBySlug(slug: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/slug/${slug}`);
  }

  rateProduct(productId: number, stars: number): Observable<ProductRatingSummary> {
    return this.http.put<ProductRatingSummary>(`${this.baseUrl}/${productId}/rating`, { stars });
  }
}
