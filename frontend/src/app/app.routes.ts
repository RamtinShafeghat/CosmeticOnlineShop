import { Routes } from '@angular/router';
import { authGuard } from './core/auth.guard';
import { AccountAddressesComponent } from './pages/account-addresses/account-addresses.component';
import { AccountOrderDetailComponent } from './pages/account-order-detail/account-order-detail.component';
import { AccountOrdersComponent } from './pages/account-orders/account-orders.component';
import { AccountWishlistComponent } from './pages/account-wishlist/account-wishlist.component';
import { AccountComponent } from './pages/account/account.component';
import { CartComponent } from './pages/cart/cart.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { OrderConfirmationComponent } from './pages/order-confirmation/order-confirmation.component';
import { ProductDetailComponent } from './pages/product-detail/product-detail.component';
import { RegisterComponent } from './pages/register/register.component';
import { ShopComponent } from './pages/shop/shop.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'shop', component: ShopComponent },
  { path: 'shop/:categorySlug', component: ShopComponent },
  { path: 'product/:slug', component: ProductDetailComponent },
  { path: 'cart', component: CartComponent },
  { path: 'checkout', component: CheckoutComponent },
  { path: 'order/:id', component: OrderConfirmationComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'account',
    component: AccountComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'orders' },
      { path: 'orders', component: AccountOrdersComponent },
      { path: 'orders/:id', component: AccountOrderDetailComponent },
      { path: 'addresses', component: AccountAddressesComponent },
      { path: 'wishlist', component: AccountWishlistComponent }
    ]
  },
  { path: '**', redirectTo: '' }
];
