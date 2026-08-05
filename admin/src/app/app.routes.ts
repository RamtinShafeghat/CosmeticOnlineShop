import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { CategoriesComponent } from './pages/categories/categories.component';
import { CategoryFormComponent } from './pages/category-form/category-form.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { LoginComponent } from './pages/login/login.component';
import { OrderDetailComponent } from './pages/order-detail/order-detail.component';
import { OrdersComponent } from './pages/orders/orders.component';
import { ProductFormComponent } from './pages/product-form/product-form.component';
import { ProductsComponent } from './pages/products/products.component';
import { AdminShellComponent } from './shared/admin-shell/admin-shell.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: AdminShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', component: DashboardComponent },
      { path: 'categories', component: CategoriesComponent },
      { path: 'categories/new', component: CategoryFormComponent },
      { path: 'categories/:id', component: CategoryFormComponent },
      { path: 'products', component: ProductsComponent },
      { path: 'products/new', component: ProductFormComponent },
      { path: 'products/:id', component: ProductFormComponent },
      { path: 'orders', component: OrdersComponent },
      { path: 'orders/:id', component: OrderDetailComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];
