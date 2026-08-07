import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AdminCustomerDetail,
  AdminCustomerListItem,
  AdminLoginResponse,
  AdminOrderListItem,
  Category,
  Order,
  Product,
  UpdateCustomer,
  UpsertCategory,
  UpsertProduct
} from './models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  login(email: string, password: string): Observable<AdminLoginResponse> {
    return this.http.post<AdminLoginResponse>(`${this.base}/admin/auth/login`, {
      email,
      password
    });
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.base}/admin/categories`);
  }

  getCategory(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.base}/admin/categories/${id}`);
  }

  createCategory(payload: UpsertCategory): Observable<Category> {
    return this.http.post<Category>(`${this.base}/admin/categories`, payload);
  }

  updateCategory(id: number, payload: UpsertCategory): Observable<Category> {
    return this.http.put<Category>(`${this.base}/admin/categories/${id}`, payload);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/admin/categories/${id}`);
  }

  getProducts(categoryId?: number): Observable<Product[]> {
    const params = categoryId != null ? `?categoryId=${categoryId}` : '';
    return this.http.get<Product[]>(`${this.base}/admin/products${params}`);
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.base}/admin/products/${id}`);
  }

  createProduct(payload: UpsertProduct): Observable<Product> {
    return this.http.post<Product>(`${this.base}/admin/products`, payload);
  }

  updateProduct(id: number, payload: UpsertProduct): Observable<Product> {
    return this.http.put<Product>(`${this.base}/admin/products/${id}`, payload);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/admin/products/${id}`);
  }

  uploadProductImage(id: number, file: File): Observable<{ imageUrl: string }> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<{ imageUrl: string }>(
      `${this.base}/admin/products/${id}/image`,
      form
    );
  }

  getOrders(status?: string): Observable<AdminOrderListItem[]> {
    const params = status ? `?status=${encodeURIComponent(status)}` : '';
    return this.http.get<AdminOrderListItem[]>(`${this.base}/admin/orders${params}`);
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.base}/admin/orders/${id}`);
  }

  confirmOrder(id: number): Observable<Order> {
    return this.http.post<Order>(`${this.base}/admin/orders/${id}/confirm`, {});
  }

  getCustomers(search?: string): Observable<AdminCustomerListItem[]> {
    const params = search ? `?search=${encodeURIComponent(search)}` : '';
    return this.http.get<AdminCustomerListItem[]>(`${this.base}/admin/customers${params}`);
  }

  getCustomer(id: number): Observable<AdminCustomerDetail> {
    return this.http.get<AdminCustomerDetail>(`${this.base}/admin/customers/${id}`);
  }

  updateCustomer(id: number, payload: UpdateCustomer): Observable<AdminCustomerDetail> {
    return this.http.put<AdminCustomerDetail>(`${this.base}/admin/customers/${id}`, payload);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/admin/customers/${id}`);
  }
}
