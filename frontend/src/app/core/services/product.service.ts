import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product } from '../models/shop.models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/products`;

  getProducts(options?: {
    categoryId?: number;
    search?: string;
    featured?: boolean;
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
    return this.http.get<Product[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  getBySlug(slug: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/slug/${slug}`);
  }
}
