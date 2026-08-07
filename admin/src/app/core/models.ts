export interface AdminLoginResponse {
  token: string;
  email: string;
  displayName: string;
  expiresAtUtc: string;
}

export interface Category {
  id: number;
  name: string;
  nameFa: string;
  slug: string;
  description: string;
  descriptionFa: string;
  productCount: number;
}

export interface Product {
  id: number;
  name: string;
  nameFa: string;
  slug: string;
  description?: string;
  descriptionFa?: string;
  shortDescription: string;
  shortDescriptionFa: string;
  price: number;
  imageUrl: string;
  brand: string;
  skinType: string;
  stock: number;
  isFeatured: boolean;
  categoryId: number;
  categoryName: string;
  categoryNameFa: string;
  averageRating?: number;
  ratingCount?: number;
  myRating?: number | null;
}

export interface UpsertCategory {
  name: string;
  nameFa: string;
  slug?: string;
  description: string;
  descriptionFa: string;
}

export interface UpsertProduct {
  name: string;
  nameFa: string;
  slug?: string;
  description: string;
  descriptionFa: string;
  shortDescription: string;
  shortDescriptionFa: string;
  price: number;
  imageUrl?: string;
  brand: string;
  skinType: string;
  stock: number;
  isFeatured: boolean;
  categoryId: number;
}

export interface AdminOrderListItem {
  id: number;
  customerName: string;
  email: string;
  status: string;
  total: number;
  itemCount: number;
  createdAt: string;
}

export interface AdminCustomerListItem {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  createdAt: string;
  orderCount: number;
  totalSpent: number;
}

export interface CustomerAddress {
  id: number;
  label: string;
  fullName: string;
  phone: string;
  line1: string;
  city: string;
  postalCode: string;
  isDefault: boolean;
}

export interface AdminCustomerDetail {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  createdAt: string;
  orderCount: number;
  totalSpent: number;
  addresses: CustomerAddress[];
  orders: AdminOrderListItem[];
}

export interface UpdateCustomer {
  fullName: string;
  email: string;
  phone: string;
  newPassword?: string;
}

export interface OrderItem {
  productId: number;
  productName: string;
  productNameFa: string;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  publicToken?: string;
  customerName: string;
  email: string;
  phone: string;
  shippingAddress: string;
  city: string;
  postalCode: string;
  status: string;
  subtotal: number;
  shippingCost: number;
  total: number;
  createdAt: string;
  items: OrderItem[];
}
