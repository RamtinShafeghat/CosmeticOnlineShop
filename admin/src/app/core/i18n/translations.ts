export type Lang = 'en' | 'fa';

export type TranslationKey =
  | 'lang.en'
  | 'lang.fa'
  | 'common.admin'
  | 'common.back'
  | 'common.save'
  | 'common.saving'
  | 'common.edit'
  | 'common.delete'
  | 'common.id'
  | 'common.customer'
  | 'common.status'
  | 'common.total'
  | 'common.email'
  | 'common.password'
  | 'common.loading'
  | 'common.featured'
  | 'common.errorGeneric'
  | 'nav.dashboard'
  | 'nav.categories'
  | 'nav.products'
  | 'nav.orders'
  | 'nav.signOut'
  | 'shell.sub'
  | 'shell.eyebrow'
  | 'shell.greeting'
  | 'shell.live'
  | 'login.headline'
  | 'login.lead'
  | 'login.access'
  | 'login.title'
  | 'login.hint'
  | 'login.submit'
  | 'login.submitting'
  | 'login.failed'
  | 'dashboard.eyebrow'
  | 'dashboard.title'
  | 'dashboard.lead'
  | 'dashboard.newProduct'
  | 'dashboard.viewOrders'
  | 'dashboard.loadError'
  | 'dashboard.categories'
  | 'dashboard.products'
  | 'dashboard.orders'
  | 'dashboard.lowStock'
  | 'dashboard.manageCategories'
  | 'dashboard.manageProducts'
  | 'dashboard.openOrders'
  | 'dashboard.reviewInventory'
  | 'dashboard.featuredMeta'
  | 'dashboard.revenueMeta'
  | 'dashboard.lowStockMeta'
  | 'dashboard.featuredEdit'
  | 'dashboard.catalogPulse'
  | 'dashboard.featuredCopy'
  | 'dashboard.categoriesCopy'
  | 'dashboard.recentOrders'
  | 'dashboard.openAll'
  | 'dashboard.noOrders'
  | 'categories.eyebrow'
  | 'categories.title'
  | 'categories.lead'
  | 'categories.new'
  | 'categories.name'
  | 'categories.persian'
  | 'categories.slug'
  | 'categories.products'
  | 'categories.loadError'
  | 'categories.deleteConfirm'
  | 'categories.deleted'
  | 'categories.deleteFailed'
  | 'categoryForm.back'
  | 'categoryForm.eyebrow'
  | 'categoryForm.new'
  | 'categoryForm.edit'
  | 'categoryForm.nameEn'
  | 'categoryForm.nameFa'
  | 'categoryForm.slug'
  | 'categoryForm.descEn'
  | 'categoryForm.descFa'
  | 'categoryForm.notFound'
  | 'categoryForm.saveFailed'
  | 'products.eyebrow'
  | 'products.title'
  | 'products.lead'
  | 'products.new'
  | 'products.image'
  | 'products.name'
  | 'products.category'
  | 'products.price'
  | 'products.stock'
  | 'products.loadError'
  | 'products.deleteConfirm'
  | 'products.deleted'
  | 'products.deleteFailed'
  | 'productForm.back'
  | 'productForm.eyebrow'
  | 'productForm.new'
  | 'productForm.edit'
  | 'productForm.nameEn'
  | 'productForm.nameFa'
  | 'productForm.slug'
  | 'productForm.category'
  | 'productForm.price'
  | 'productForm.stock'
  | 'productForm.brand'
  | 'productForm.skinType'
  | 'productForm.featured'
  | 'productForm.shortEn'
  | 'productForm.shortFa'
  | 'productForm.descEn'
  | 'productForm.descFa'
  | 'productForm.imageFile'
  | 'productForm.imageUrl'
  | 'productForm.chooseCategory'
  | 'productForm.stockRequired'
  | 'productForm.notFound'
  | 'productForm.saveFailed'
  | 'productForm.uploadFailed'
  | 'productForm.uploading'
  | 'productForm.loadCategoriesFailed'
  | 'skin.All'
  | 'skin.Dry'
  | 'skin.Normal'
  | 'skin.Sensitive'
  | 'orders.eyebrow'
  | 'orders.title'
  | 'orders.lead'
  | 'orders.items'
  | 'orders.placed'
  | 'orders.empty'
  | 'orders.emptyPending'
  | 'orders.emptyConfirmed'
  | 'orders.loadError'
  | 'orders.tabPending'
  | 'orders.tabConfirmed'
  | 'orders.confirm'
  | 'orderDetail.back'
  | 'orderDetail.eyebrow'
  | 'orderDetail.title'
  | 'orderDetail.customer'
  | 'orderDetail.items'
  | 'orderDetail.subtotal'
  | 'orderDetail.shipping'
  | 'orderDetail.total'
  | 'orderDetail.notFound'
  | 'orderDetail.loading'
  | 'orderDetail.confirm'
  | 'orderDetail.confirming'
  | 'orderDetail.confirmFailed'
  | 'orderDetail.confirmedNote'
  | 'status.Confirmed'
  | 'status.Pending';

export const TRANSLATIONS: Record<Lang, Record<TranslationKey, string>> = {
  en: {
    'lang.en': 'EN',
    'lang.fa': 'FA',
    'common.admin': 'Admin',
    'common.back': 'Back',
    'common.save': 'Save',
    'common.saving': 'Saving…',
    'common.edit': 'Edit',
    'common.delete': 'Delete',
    'common.id': 'ID',
    'common.customer': 'Customer',
    'common.status': 'Status',
    'common.total': 'Total',
    'common.email': 'Email',
    'common.password': 'Password',
    'common.loading': 'Loading…',
    'common.featured': 'Featured',
    'common.errorGeneric': 'Something went wrong.',
    'nav.dashboard': 'Dashboard',
    'nav.categories': 'Categories',
    'nav.products': 'Products',
    'nav.orders': 'Orders',
    'nav.signOut': 'Sign out',
    'shell.sub': 'Studio Admin',
    'shell.eyebrow': 'Velora control room',
    'shell.greeting': 'Keep the catalog luminous.',
    'shell.live': 'Live storefront connected',
    'login.headline': 'Beauty operations, quietly refined.',
    'login.lead':
      'Shape the catalog, refresh imagery, and follow every order from one calm studio.',
    'login.access': 'Admin access',
    'login.title': 'Sign in',
    'login.hint': 'Use your Velora studio credentials to continue.',
    'login.submit': 'Enter studio',
    'login.submitting': 'Signing in…',
    'login.failed': 'Login failed. Check your credentials.',
    'dashboard.eyebrow': 'Today in the studio',
    'dashboard.title': 'Dashboard',
    'dashboard.lead':
      'A calm overview of catalog health, featured rituals, and the latest customer orders.',
    'dashboard.newProduct': 'New product',
    'dashboard.viewOrders': 'View orders',
    'dashboard.loadError': 'Unable to load dashboard data.',
    'dashboard.categories': 'Categories',
    'dashboard.products': 'Products',
    'dashboard.orders': 'Orders',
    'dashboard.lowStock': 'Low stock',
    'dashboard.manageCategories': 'Manage categories',
    'dashboard.manageProducts': 'Manage products',
    'dashboard.openOrders': 'Open orders',
    'dashboard.reviewInventory': 'Review inventory',
    'dashboard.featuredMeta': '{count} featured',
    'dashboard.revenueMeta': '{amount} total',
    'dashboard.lowStockMeta': 'Below 20 units',
    'dashboard.featuredEdit': 'Featured edit',
    'dashboard.catalogPulse': 'Catalog pulse',
    'dashboard.featuredCopy': 'products currently featured on the storefront.',
    'dashboard.categoriesCopy': 'active categories shaping the browse experience.',
    'dashboard.recentOrders': 'Recent orders',
    'dashboard.openAll': 'Open all',
    'dashboard.noOrders':
      'No orders yet. When customers check out, they’ll appear here.',
    'categories.eyebrow': 'Catalog structure',
    'categories.title': 'Categories',
    'categories.lead': 'Shape browse paths in English and Persian.',
    'categories.new': 'New category',
    'categories.name': 'Name',
    'categories.persian': 'Persian',
    'categories.slug': 'Slug',
    'categories.products': 'Products',
    'categories.loadError': 'Unable to load categories.',
    'categories.deleteConfirm': 'Delete category "{name}"?',
    'categories.deleted': 'Deleted {name}.',
    'categories.deleteFailed': 'Delete failed.',
    'categoryForm.back': '← Back to categories',
    'categoryForm.eyebrow': 'Category editor',
    'categoryForm.new': 'New category',
    'categoryForm.edit': 'Edit category',
    'categoryForm.nameEn': 'Name (EN)',
    'categoryForm.nameFa': 'Name (FA)',
    'categoryForm.slug': 'Slug (optional)',
    'categoryForm.descEn': 'Description (EN)',
    'categoryForm.descFa': 'Description (FA)',
    'categoryForm.notFound': 'Category not found.',
    'categoryForm.saveFailed': 'Save failed.',
    'products.eyebrow': 'Merchandising',
    'products.title': 'Products',
    'products.lead': 'Curate rituals, pricing, stock, and imagery.',
    'products.new': 'New product',
    'products.image': 'Image',
    'products.name': 'Name',
    'products.category': 'Category',
    'products.price': 'Price',
    'products.stock': 'Stock',
    'products.loadError': 'Unable to load products.',
    'products.deleteConfirm': 'Delete product "{name}"?',
    'products.deleted': 'Deleted {name}.',
    'products.deleteFailed': 'Delete failed.',
    'productForm.back': '← Back to products',
    'productForm.eyebrow': 'Product editor',
    'productForm.new': 'New product',
    'productForm.edit': 'Edit product',
    'productForm.nameEn': 'Name (EN)',
    'productForm.nameFa': 'Name (FA)',
    'productForm.slug': 'Slug (optional)',
    'productForm.category': 'Category',
    'productForm.price': 'Price',
    'productForm.stock': 'Stock',
    'productForm.brand': 'Brand',
    'productForm.skinType': 'Skin type',
    'productForm.featured': 'Featured on the storefront',
    'productForm.shortEn': 'Short description (EN)',
    'productForm.shortFa': 'Short description (FA)',
    'productForm.descEn': 'Description (EN)',
    'productForm.descFa': 'Description (FA)',
    'productForm.imageFile': 'Product image (from your computer)',
    'productForm.imageUrl': 'Or image URL',
    'productForm.chooseCategory': 'Please choose a category.',
    'productForm.stockRequired': 'Please enter a valid stock quantity (0 or more).',
    'productForm.notFound': 'Product not found.',
    'productForm.saveFailed': 'Save failed.',
    'productForm.uploadFailed':
      'Product saved, but image upload failed. You can try uploading again.',
    'productForm.uploading': 'Uploading image…',
    'productForm.loadCategoriesFailed': 'Unable to load categories.',
    'skin.All': 'All',
    'skin.Dry': 'Dry',
    'skin.Normal': 'Normal',
    'skin.Sensitive': 'Sensitive',
    'orders.eyebrow': 'Fulfillment',
    'orders.title': 'Orders',
    'orders.lead': 'Review pending checkouts, then confirm them for the customer.',
    'orders.items': 'Items',
    'orders.placed': 'Placed',
    'orders.empty': 'No orders yet.',
    'orders.emptyPending': 'No pending orders right now.',
    'orders.emptyConfirmed': 'No confirmed orders yet.',
    'orders.loadError': 'Unable to load orders.',
    'orders.tabPending': 'Pending',
    'orders.tabConfirmed': 'Confirmed',
    'orders.confirm': 'Confirm',
    'orderDetail.back': '← Back to orders',
    'orderDetail.eyebrow': 'Order detail',
    'orderDetail.title': 'Order #{id}',
    'orderDetail.customer': 'Customer',
    'orderDetail.items': 'Items',
    'orderDetail.subtotal': 'Subtotal',
    'orderDetail.shipping': 'Shipping',
    'orderDetail.total': 'Total',
    'orderDetail.notFound': 'Order not found.',
    'orderDetail.loading': 'Loading order…',
    'orderDetail.confirm': 'Confirm order',
    'orderDetail.confirming': 'Confirming…',
    'orderDetail.confirmFailed': 'Could not confirm this order.',
    'orderDetail.confirmedNote': 'This order is confirmed and visible as confirmed to the customer.',
    'status.Confirmed': 'Confirmed',
    'status.Pending': 'Pending'
  },
  fa: {
    'lang.en': 'EN',
    'lang.fa': 'فا',
    'common.admin': 'مدیر',
    'common.back': 'بازگشت',
    'common.save': 'ذخیره',
    'common.saving': 'در حال ذخیره…',
    'common.edit': 'ویرایش',
    'common.delete': 'حذف',
    'common.id': 'شناسه',
    'common.customer': 'مشتری',
    'common.status': 'وضعیت',
    'common.total': 'جمع',
    'common.email': 'ایمیل',
    'common.password': 'رمز عبور',
    'common.loading': 'در حال بارگذاری…',
    'common.featured': 'ویژه',
    'common.errorGeneric': 'خطایی رخ داد.',
    'nav.dashboard': 'داشبورد',
    'nav.categories': 'دسته‌بندی‌ها',
    'nav.products': 'محصولات',
    'nav.orders': 'سفارش‌ها',
    'nav.signOut': 'خروج',
    'shell.sub': 'پنل مدیریت',
    'shell.eyebrow': 'اتاق کنترل ولورا',
    'shell.greeting': 'کاتالوگ را درخشان نگه دارید.',
    'shell.live': 'فروشگاه آنلاین متصل است',
    'login.headline': 'عملیات زیبایی، آرام و دقیق.',
    'login.lead':
      'کاتالوگ را شکل دهید، تصاویر را تازه کنید و همه سفارش‌ها را از یک استودیو دنبال کنید.',
    'login.access': 'ورود مدیر',
    'login.title': 'ورود',
    'login.hint': 'با اطلاعات استودیو ولورا ادامه دهید.',
    'login.submit': 'ورود به استودیو',
    'login.submitting': 'در حال ورود…',
    'login.failed': 'ورود ناموفق بود. اطلاعات را بررسی کنید.',
    'dashboard.eyebrow': 'امروز در استودیو',
    'dashboard.title': 'داشبورد',
    'dashboard.lead':
      'نگاهی آرام به سلامت کاتالوگ، محصولات ویژه و آخرین سفارش‌های مشتریان.',
    'dashboard.newProduct': 'محصول جدید',
    'dashboard.viewOrders': 'مشاهده سفارش‌ها',
    'dashboard.loadError': 'بارگذاری داده‌های داشبورد ممکن نشد.',
    'dashboard.categories': 'دسته‌بندی‌ها',
    'dashboard.products': 'محصولات',
    'dashboard.orders': 'سفارش‌ها',
    'dashboard.lowStock': 'موجودی کم',
    'dashboard.manageCategories': 'مدیریت دسته‌بندی‌ها',
    'dashboard.manageProducts': 'مدیریت محصولات',
    'dashboard.openOrders': 'باز کردن سفارش‌ها',
    'dashboard.reviewInventory': 'بررسی موجودی',
    'dashboard.featuredMeta': '{count} ویژه',
    'dashboard.revenueMeta': 'جمع {amount}',
    'dashboard.lowStockMeta': 'کمتر از ۲۰ واحد',
    'dashboard.featuredEdit': 'گزینه ویژه',
    'dashboard.catalogPulse': 'نبض کاتالوگ',
    'dashboard.featuredCopy': 'محصول اکنون در فروشگاه ویژه شده‌اند.',
    'dashboard.categoriesCopy': 'دسته‌بندی فعال مسیر مرور را شکل می‌دهند.',
    'dashboard.recentOrders': 'سفارش‌های اخیر',
    'dashboard.openAll': 'مشاهده همه',
    'dashboard.noOrders':
      'هنوز سفارشی ثبت نشده است. پس از خرید مشتریان، اینجا دیده می‌شوند.',
    'categories.eyebrow': 'ساختار کاتالوگ',
    'categories.title': 'دسته‌بندی‌ها',
    'categories.lead': 'مسیرهای مرور را به انگلیسی و فارسی شکل دهید.',
    'categories.new': 'دسته‌بندی جدید',
    'categories.name': 'نام',
    'categories.persian': 'فارسی',
    'categories.slug': 'اسلاگ',
    'categories.products': 'محصولات',
    'categories.loadError': 'بارگذاری دسته‌بندی‌ها ممکن نشد.',
    'categories.deleteConfirm': 'دسته‌بندی «{name}» حذف شود؟',
    'categories.deleted': '«{name}» حذف شد.',
    'categories.deleteFailed': 'حذف ناموفق بود.',
    'categoryForm.back': '→ بازگشت به دسته‌بندی‌ها',
    'categoryForm.eyebrow': 'ویرایشگر دسته‌بندی',
    'categoryForm.new': 'دسته‌بندی جدید',
    'categoryForm.edit': 'ویرایش دسته‌بندی',
    'categoryForm.nameEn': 'نام (انگلیسی)',
    'categoryForm.nameFa': 'نام (فارسی)',
    'categoryForm.slug': 'اسلاگ (اختیاری)',
    'categoryForm.descEn': 'توضیحات (انگلیسی)',
    'categoryForm.descFa': 'توضیحات (فارسی)',
    'categoryForm.notFound': 'دسته‌بندی پیدا نشد.',
    'categoryForm.saveFailed': 'ذخیره ناموفق بود.',
    'products.eyebrow': 'مرچندایزینگ',
    'products.title': 'محصولات',
    'products.lead': 'آیین‌ها، قیمت، موجودی و تصاویر را مدیریت کنید.',
    'products.new': 'محصول جدید',
    'products.image': 'تصویر',
    'products.name': 'نام',
    'products.category': 'دسته‌بندی',
    'products.price': 'قیمت',
    'products.stock': 'موجودی',
    'products.loadError': 'بارگذاری محصولات ممکن نشد.',
    'products.deleteConfirm': 'محصول «{name}» حذف شود؟',
    'products.deleted': '«{name}» حذف شد.',
    'products.deleteFailed': 'حذف ناموفق بود.',
    'productForm.back': '→ بازگشت به محصولات',
    'productForm.eyebrow': 'ویرایشگر محصول',
    'productForm.new': 'محصول جدید',
    'productForm.edit': 'ویرایش محصول',
    'productForm.nameEn': 'نام (انگلیسی)',
    'productForm.nameFa': 'نام (فارسی)',
    'productForm.slug': 'اسلاگ (اختیاری)',
    'productForm.category': 'دسته‌بندی',
    'productForm.price': 'قیمت',
    'productForm.stock': 'موجودی',
    'productForm.brand': 'برند',
    'productForm.skinType': 'نوع پوست',
    'productForm.featured': 'نمایش ویژه در فروشگاه',
    'productForm.shortEn': 'توضیح کوتاه (انگلیسی)',
    'productForm.shortFa': 'توضیح کوتاه (فارسی)',
    'productForm.descEn': 'توضیحات (انگلیسی)',
    'productForm.descFa': 'توضیحات (فارسی)',
    'productForm.imageFile': 'تصویر محصول (از رایانه شما)',
    'productForm.imageUrl': 'یا آدرس تصویر',
    'productForm.chooseCategory': 'لطفاً یک دسته‌بندی انتخاب کنید.',
    'productForm.stockRequired': 'لطفاً موجودی معتبر وارد کنید (۰ یا بیشتر).',
    'productForm.notFound': 'محصول پیدا نشد.',
    'productForm.saveFailed': 'ذخیره ناموفق بود.',
    'productForm.uploadFailed':
      'محصول ذخیره شد، اما آپلود تصویر ناموفق بود. دوباره تلاش کنید.',
    'productForm.uploading': 'در حال آپلود تصویر…',
    'productForm.loadCategoriesFailed': 'بارگذاری دسته‌بندی‌ها ممکن نشد.',
    'skin.All': 'همه',
    'skin.Dry': 'خشک',
    'skin.Normal': 'معمولی',
    'skin.Sensitive': 'حساس',
    'orders.eyebrow': 'تحویل سفارش',
    'orders.title': 'سفارش‌ها',
    'orders.lead': 'سفارش‌های در انتظار را بررسی کنید و سپس برای مشتری تأیید کنید.',
    'orders.items': 'اقلام',
    'orders.placed': 'زمان ثبت',
    'orders.empty': 'هنوز سفارشی ثبت نشده است.',
    'orders.emptyPending': 'در حال حاضر سفارش در انتظاری نیست.',
    'orders.emptyConfirmed': 'هنوز سفارش تأییدشده‌ای نیست.',
    'orders.loadError': 'بارگذاری سفارش‌ها ممکن نشد.',
    'orders.tabPending': 'در انتظار',
    'orders.tabConfirmed': 'تأیید شده',
    'orders.confirm': 'تأیید',
    'orderDetail.back': '→ بازگشت به سفارش‌ها',
    'orderDetail.eyebrow': 'جزئیات سفارش',
    'orderDetail.title': 'سفارش #{id}',
    'orderDetail.customer': 'مشتری',
    'orderDetail.items': 'اقلام',
    'orderDetail.subtotal': 'جمع جزء',
    'orderDetail.shipping': 'ارسال',
    'orderDetail.total': 'جمع کل',
    'orderDetail.notFound': 'سفارش پیدا نشد.',
    'orderDetail.loading': 'در حال بارگذاری سفارش…',
    'orderDetail.confirm': 'تأیید سفارش',
    'orderDetail.confirming': 'در حال تأیید…',
    'orderDetail.confirmFailed': 'تأیید این سفارش ممکن نشد.',
    'orderDetail.confirmedNote': 'این سفارش تأیید شده و برای مشتری به‌صورت تأیید شده نمایش داده می‌شود.',
    'status.Confirmed': 'تأیید شده',
    'status.Pending': 'در انتظار'
  }
};
