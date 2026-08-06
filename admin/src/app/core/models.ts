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
