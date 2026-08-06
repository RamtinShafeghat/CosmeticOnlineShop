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
  shortDescription: string;
  shortDescriptionFa: string;
  description?: string;
  descriptionFa?: string;
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

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface CreateOrderItem {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  customerName: string;
  email: string;
  phone: string;
  shippingAddress: string;
  city: string;
  postalCode: string;
  saveAddress?: boolean;
  addressLabel?: string;
  items: CreateOrderItem[];
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

export interface CustomerLoginRequest {
  email: string;
  password: string;
}

export interface CustomerRegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
}

export interface CustomerAuthResponse {
  token: string;
  email: string;
  fullName: string;
  phone: string;
  expiresAtUtc: string;
}

export interface CustomerProfile {
  email: string;
  fullName: string;
  phone: string;
  expiresAtUtc: string;
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

export interface UpsertCustomerAddress {
  label: string;
  fullName: string;
  phone: string;
  line1: string;
  city: string;
  postalCode: string;
  isDefault: boolean;
}

export interface CustomerOrderListItem {
  id: number;
  status: string;
  total: number;
  itemCount: number;
  createdAt: string;
}
