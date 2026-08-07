export type Lang = 'en' | 'fa';

export type TranslationKey =
  | 'nav.home'
  | 'nav.shop'
  | 'nav.bag'
  | 'nav.signIn'
  | 'nav.account'
  | 'lang.en'
  | 'lang.fa'
  | 'home.headline'
  | 'home.lead'
  | 'home.ctaShop'
  | 'home.ctaSkincare'
  | 'home.featuredTitle'
  | 'home.featuredLead'
  | 'home.ritualTitle'
  | 'home.ritualLead'
  | 'home.browseAll'
  | 'home.loading'
  | 'home.error'
  | 'shop.title'
  | 'shop.lead'
  | 'shop.searchPlaceholder'
  | 'shop.search'
  | 'shop.all'
  | 'shop.loading'
  | 'shop.error'
  | 'shop.empty'
  | 'shop.categoryMissing'
  | 'shop.brand'
  | 'shop.skinTypeFilter'
  | 'shop.allBrands'
  | 'shop.allSkinTypes'
  | 'product.addToBag'
  | 'product.viewBag'
  | 'product.added'
  | 'product.qty'
  | 'product.skinType'
  | 'product.inStock'
  | 'product.loading'
  | 'product.notFound'
  | 'product.backToShop'
  | 'product.ratingAria'
  | 'product.ratingNone'
  | 'product.ratingOne'
  | 'product.ratingMany'
  | 'product.ratePrompt'
  | 'product.yourRating'
  | 'product.signInToRate'
  | 'product.ratingSaved'
  | 'product.ratingFailed'
  | 'cart.title'
  | 'cart.empty'
  | 'cart.continue'
  | 'cart.summary'
  | 'cart.subtotal'
  | 'cart.shipping'
  | 'cart.free'
  | 'cart.total'
  | 'cart.note'
  | 'cart.checkout'
  | 'cart.remove'
  | 'cart.qty'
  | 'checkout.title'
  | 'checkout.empty'
  | 'checkout.continue'
  | 'checkout.fullName'
  | 'checkout.email'
  | 'checkout.phone'
  | 'checkout.address'
  | 'checkout.city'
  | 'checkout.postal'
  | 'checkout.placeOrder'
  | 'checkout.placing'
  | 'checkout.summary'
  | 'checkout.bagEmpty'
  | 'checkout.failed'
  | 'checkout.loginHint'
  | 'checkout.signIn'
  | 'checkout.savedAddress'
  | 'checkout.manualAddress'
  | 'checkout.saveAddress'
  | 'auth.access'
  | 'auth.email'
  | 'auth.password'
  | 'auth.fullName'
  | 'auth.phone'
  | 'login.title'
  | 'login.lead'
  | 'login.submit'
  | 'login.submitting'
  | 'login.failed'
  | 'login.noAccount'
  | 'login.registerLink'
  | 'register.title'
  | 'register.lead'
  | 'register.submit'
  | 'register.submitting'
  | 'register.failed'
  | 'register.hasAccount'
  | 'register.loginLink'
  | 'account.eyebrow'
  | 'account.title'
  | 'account.greeting'
  | 'account.signOut'
  | 'account.orders'
  | 'account.addresses'
  | 'account.wishlist'
  | 'account.loadingWishlist'
  | 'account.noWishlist'
  | 'wishlist.add'
  | 'wishlist.remove'
  | 'account.loadingOrders'
  | 'account.ordersError'
  | 'account.noOrders'
  | 'account.shopNow'
  | 'account.items'
  | 'account.backOrders'
  | 'account.orderTitle'
  | 'account.savedAddresses'
  | 'account.loadingAddresses'
  | 'account.noAddresses'
  | 'account.default'
  | 'account.edit'
  | 'account.delete'
  | 'account.addAddress'
  | 'account.editAddress'
  | 'account.label'
  | 'account.setDefault'
  | 'account.saveAddress'
  | 'account.saving'
  | 'account.cancel'
  | 'account.addressSaveFailed'
  | 'account.addressDeleteFailed'
  | 'account.addressesError'
  | 'order.loading'
  | 'order.notFound'
  | 'order.back'
  | 'order.thanks'
  | 'order.confirmed'
  | 'order.pendingTitle'
  | 'order.receipt'
  | 'order.receiptPending'
  | 'order.status'
  | 'order.placed'
  | 'order.shipTo'
  | 'order.shipping'
  | 'order.free'
  | 'order.total'
  | 'order.continue'
  | 'order.status.Confirmed'
  | 'order.status.Pending'
  | 'footer.tagline'
  | 'footer.shopAll'
  | 'footer.skincare'
  | 'footer.makeup'
  | 'footer.legal'
  | 'skin.All'
  | 'skin.Dry'
  | 'skin.Normal'
  | 'skin.Sensitive';

export const TRANSLATIONS: Record<Lang, Record<TranslationKey, string>> = {
  en: {
    'nav.home': 'Home',
    'nav.shop': 'Shop',
    'nav.bag': 'Bag',
    'nav.signIn': 'Sign in',
    'nav.account': 'Account',
    'lang.en': 'EN',
    'lang.fa': 'FA',
    'home.headline': 'Skin that feels quietly luminous.',
    'home.lead':
      'Refined skincare and soft-color makeup crafted for everyday radiance.',
    'home.ctaShop': 'Shop the collection',
    'home.ctaSkincare': 'Explore skincare',
    'home.featuredTitle': 'Featured rituals',
    'home.featuredLead':
      'A curated edit of Velora essentials—serums, soft color, and scent.',
    'home.ritualTitle': 'Beauty, unhurried.',
    'home.ritualLead':
      'Velora formulas favor clean textures, soft finishes, and ingredients that earn their place in a daily ritual.',
    'home.browseAll': 'Browse all products',
    'home.loading': 'Loading the collection…',
    'home.error': 'Unable to load featured products. Is the API running?',
    'shop.title': 'Shop',
    'shop.lead': 'Discover Velora skincare, makeup, fragrance, and body care.',
    'shop.searchPlaceholder': 'Search products',
    'shop.search': 'Search',
    'shop.all': 'All',
    'shop.loading': 'Loading products…',
    'shop.error': 'Unable to load products. Is the API running?',
    'shop.empty': 'No products match your filters.',
    'shop.categoryMissing': 'Category not found.',
    'shop.brand': 'Brand',
    'shop.skinTypeFilter': 'Skin type',
    'shop.allBrands': 'All brands',
    'shop.allSkinTypes': 'All skin types',
    'product.addToBag': 'Add to bag',
    'product.viewBag': 'View bag',
    'product.added': 'Added to your bag.',
    'product.qty': 'Qty',
    'product.skinType': 'Skin type',
    'product.inStock': 'In stock',
    'product.loading': 'Loading product…',
    'product.notFound': 'Product not found.',
    'product.backToShop': 'Back to shop',
    'product.ratingAria': 'Product rating',
    'product.ratingNone': 'No ratings yet',
    'product.ratingOne': 'rating',
    'product.ratingMany': 'ratings',
    'product.ratePrompt': 'Tap a star to rate this product',
    'product.yourRating': 'Your rating',
    'product.signInToRate': 'Sign in to rate this product',
    'product.ratingSaved': 'Thanks — your rating was saved.',
    'product.ratingFailed': 'Could not save your rating. Please try again.',
    'cart.title': 'Your bag',
    'cart.empty': 'Your bag is empty.',
    'cart.continue': 'Continue shopping',
    'cart.summary': 'Summary',
    'cart.subtotal': 'Subtotal',
    'cart.shipping': 'Shipping',
    'cart.free': 'Free',
    'cart.total': 'Total',
    'cart.note': 'Free shipping on orders over $75.',
    'cart.checkout': 'Checkout',
    'cart.remove': 'Remove',
    'cart.qty': 'Qty',
    'checkout.title': 'Checkout',
    'checkout.empty': 'Your bag is empty.',
    'checkout.continue': 'Continue shopping',
    'checkout.fullName': 'Full name',
    'checkout.email': 'Email',
    'checkout.phone': 'Phone',
    'checkout.address': 'Shipping address',
    'checkout.city': 'City',
    'checkout.postal': 'Postal code',
    'checkout.placeOrder': 'Place order',
    'checkout.placing': 'Placing order…',
    'checkout.summary': 'Order summary',
    'checkout.bagEmpty': 'Your bag is empty.',
    'checkout.failed': 'Could not place the order. Please try again.',
    'checkout.loginHint': 'Sign in to track purchases and reuse saved addresses.',
    'checkout.signIn': 'Sign in',
    'checkout.savedAddress': 'Saved address',
    'checkout.manualAddress': 'Enter a new address',
    'checkout.saveAddress': 'Save this address to my account',
    'auth.access': 'Your Velora account',
    'auth.email': 'Email',
    'auth.password': 'Password',
    'auth.fullName': 'Full name',
    'auth.phone': 'Phone',
    'login.title': 'Sign in',
    'login.lead': 'Access your orders, purchases, and saved addresses.',
    'login.submit': 'Sign in',
    'login.submitting': 'Signing in…',
    'login.failed': 'Invalid email or password.',
    'login.noAccount': 'New here?',
    'login.registerLink': 'Create an account',
    'register.title': 'Create account',
    'register.lead': 'Save addresses and follow every Velora order in one place.',
    'register.submit': 'Create account',
    'register.submitting': 'Creating account…',
    'register.failed': 'Could not create your account. Please try again.',
    'register.hasAccount': 'Already have an account?',
    'register.loginLink': 'Sign in',
    'account.eyebrow': 'Member studio',
    'account.title': 'My account',
    'account.greeting': 'Welcome back, {name}.',
    'account.signOut': 'Sign out',
    'account.orders': 'Orders',
    'account.addresses': 'Addresses',
    'account.wishlist': 'Wishlist',
    'account.loadingWishlist': 'Loading your wishlist…',
    'account.noWishlist': 'Your wishlist is empty. Tap the heart on any product to save it.',
    'wishlist.add': 'Save to wishlist',
    'wishlist.remove': 'Remove from wishlist',
    'account.loadingOrders': 'Loading your orders…',
    'account.ordersError': 'Unable to load orders.',
    'account.noOrders': 'You have no orders yet.',
    'account.shopNow': 'Shop now',
    'account.items': 'items',
    'account.backOrders': '← Back to orders',
    'account.orderTitle': 'Order #{id}',
    'account.savedAddresses': 'Saved addresses',
    'account.loadingAddresses': 'Loading addresses…',
    'account.noAddresses': 'No saved addresses yet.',
    'account.default': 'Default',
    'account.edit': 'Edit',
    'account.delete': 'Delete',
    'account.addAddress': 'Add address',
    'account.editAddress': 'Edit address',
    'account.label': 'Label',
    'account.setDefault': 'Set as default address',
    'account.saveAddress': 'Save address',
    'account.saving': 'Saving…',
    'account.cancel': 'Cancel',
    'account.addressSaveFailed': 'Could not save the address.',
    'account.addressDeleteFailed': 'Could not delete the address.',
    'account.addressesError': 'Unable to load addresses.',
    'order.loading': 'Loading order…',
    'order.notFound': 'Order not found.',
    'order.back': 'Back to shop',
    'order.thanks': 'Thank you',
    'order.confirmed': 'Order #{id} confirmed',
    'order.pendingTitle': 'Order #{id} received',
    'order.receipt':
      'We’ve emailed a receipt to {email}. Your Velora order has been confirmed and is being prepared.',
    'order.receiptPending':
      'We’ve received your order and sent a note to {email}. It will show as confirmed once our studio reviews it.',
    'order.status': 'Status',
    'order.placed': 'Placed',
    'order.shipTo': 'Ship to',
    'order.shipping': 'Shipping',
    'order.free': 'Free',
    'order.total': 'Total',
    'order.continue': 'Continue shopping',
    'order.status.Confirmed': 'Confirmed',
    'order.status.Pending': 'Pending',
    'footer.tagline': 'Quiet luxury for everyday skin rituals.',
    'footer.shopAll': 'Shop all',
    'footer.skincare': 'Skincare',
    'footer.makeup': 'Makeup',
    'footer.legal': '© {year} Velora Beauty',
    'skin.All': 'All',
    'skin.Dry': 'Dry',
    'skin.Normal': 'Normal',
    'skin.Sensitive': 'Sensitive'
  },
  fa: {
    'nav.home': 'خانه',
    'nav.shop': 'فروشگاه',
    'nav.bag': 'سبد',
    'nav.signIn': 'ورود',
    'nav.account': 'حساب',
    'lang.en': 'EN',
    'lang.fa': 'فا',
    'home.headline': 'پوستی که آرام می‌درخشد.',
    'home.lead':
      'مراقبت پوست ظریف و آرایش رنگ ملایم برای درخشش روزانه.',
    'home.ctaShop': 'مشاهده مجموعه',
    'home.ctaSkincare': 'کشف مراقبت پوست',
    'home.featuredTitle': 'آیین‌های ویژه',
    'home.featuredLead':
      'گزیده‌ای از ضروریات ولورا—سرم‌ها، رنگ‌های نرم و عطر.',
    'home.ritualTitle': 'زیبایی بی‌شتاب.',
    'home.ritualLead':
      'فرمول‌های ولورا بافت‌های پاک، پایان نرم و موادی را ترجیح می‌دهند که جای خود را در آیین روزانه پیدا کنند.',
    'home.browseAll': 'مشاهده همه محصولات',
    'home.loading': 'در حال بارگذاری مجموعه…',
    'home.error': 'بارگذاری محصولات ویژه ممکن نشد. آیا API در حال اجراست؟',
    'shop.title': 'فروشگاه',
    'shop.lead': 'کشف مراقبت پوست، آرایش، عطر و مراقبت بدن ولورا.',
    'shop.searchPlaceholder': 'جستجوی محصولات',
    'shop.search': 'جستجو',
    'shop.all': 'همه',
    'shop.loading': 'در حال بارگذاری محصولات…',
    'shop.error': 'بارگذاری محصولات ممکن نشد. آیا API در حال اجراست؟',
    'shop.empty': 'محصولی با این فیلترها یافت نشد.',
    'shop.categoryMissing': 'دسته‌بندی پیدا نشد.',
    'shop.brand': 'برند',
    'shop.skinTypeFilter': 'نوع پوست',
    'shop.allBrands': 'همه برندها',
    'shop.allSkinTypes': 'همه انواع پوست',
    'product.addToBag': 'افزودن به سبد',
    'product.viewBag': 'مشاهده سبد',
    'product.added': 'به سبد اضافه شد.',
    'product.qty': 'تعداد',
    'product.skinType': 'نوع پوست',
    'product.inStock': 'موجودی',
    'product.loading': 'در حال بارگذاری محصول…',
    'product.notFound': 'محصول پیدا نشد.',
    'product.backToShop': 'بازگشت به فروشگاه',
    'product.ratingAria': 'امتیاز محصول',
    'product.ratingNone': 'هنوز امتیازی ثبت نشده',
    'product.ratingOne': 'امتیاز',
    'product.ratingMany': 'امتیاز',
    'product.ratePrompt': 'برای امتیاز دادن روی ستاره‌ها بزنید',
    'product.yourRating': 'امتیاز شما',
    'product.signInToRate': 'برای امتیاز دادن وارد شوید',
    'product.ratingSaved': 'متشکریم — امتیاز شما ذخیره شد.',
    'product.ratingFailed': 'امتیاز ذخیره نشد. دوباره تلاش کنید.',
    'cart.title': 'سبد خرید شما',
    'cart.empty': 'سبد خرید شما خالی است.',
    'cart.continue': 'ادامه خرید',
    'cart.summary': 'خلاصه',
    'cart.subtotal': 'جمع جزء',
    'cart.shipping': 'ارسال',
    'cart.free': 'رایگان',
    'cart.total': 'جمع کل',
    'cart.note': 'ارسال رایگان برای سفارش‌های بالای ۷۵ دلار.',
    'cart.checkout': 'تسویه حساب',
    'cart.remove': 'حذف',
    'cart.qty': 'تعداد',
    'checkout.title': 'تسویه حساب',
    'checkout.empty': 'سبد خرید شما خالی است.',
    'checkout.continue': 'ادامه خرید',
    'checkout.fullName': 'نام کامل',
    'checkout.email': 'ایمیل',
    'checkout.phone': 'تلفن',
    'checkout.address': 'آدرس ارسال',
    'checkout.city': 'شهر',
    'checkout.postal': 'کد پستی',
    'checkout.placeOrder': 'ثبت سفارش',
    'checkout.placing': 'در حال ثبت سفارش…',
    'checkout.summary': 'خلاصه سفارش',
    'checkout.bagEmpty': 'سبد خرید شما خالی است.',
    'checkout.failed': 'ثبت سفارش ممکن نشد. لطفاً دوباره تلاش کنید.',
    'checkout.loginHint': 'برای پیگیری خریدها و استفاده از آدرس‌های ذخیره‌شده وارد شوید.',
    'checkout.signIn': 'ورود',
    'checkout.savedAddress': 'آدرس ذخیره‌شده',
    'checkout.manualAddress': 'وارد کردن آدرس جدید',
    'checkout.saveAddress': 'ذخیره این آدرس در حساب من',
    'auth.access': 'حساب ولورای شما',
    'auth.email': 'ایمیل',
    'auth.password': 'رمز عبور',
    'auth.fullName': 'نام کامل',
    'auth.phone': 'تلفن',
    'login.title': 'ورود',
    'login.lead': 'به سفارش‌ها، خریدها و آدرس‌های ذخیره‌شده دسترسی داشته باشید.',
    'login.submit': 'ورود',
    'login.submitting': 'در حال ورود…',
    'login.failed': 'ایمیل یا رمز عبور نادرست است.',
    'login.noAccount': 'تازه‌وارد هستید؟',
    'login.registerLink': 'ساخت حساب',
    'register.title': 'ساخت حساب',
    'register.lead': 'آدرس‌ها را ذخیره کنید و همه سفارش‌های ولورا را یکجا دنبال کنید.',
    'register.submit': 'ساخت حساب',
    'register.submitting': 'در حال ساخت حساب…',
    'register.failed': 'ساخت حساب ممکن نشد. لطفاً دوباره تلاش کنید.',
    'register.hasAccount': 'قبلاً حساب دارید؟',
    'register.loginLink': 'ورود',
    'account.eyebrow': 'استودیوی اعضا',
    'account.title': 'حساب من',
    'account.greeting': 'خوش آمدید، {name}.',
    'account.signOut': 'خروج',
    'account.orders': 'سفارش‌ها',
    'account.addresses': 'آدرس‌ها',
    'account.wishlist': 'علاقه‌مندی‌ها',
    'account.loadingWishlist': 'در حال بارگذاری علاقه‌مندی‌ها…',
    'account.noWishlist': 'فهرست علاقه‌مندی‌های شما خالی است. با لمس قلب روی هر محصول آن را ذخیره کنید.',
    'wishlist.add': 'افزودن به علاقه‌مندی‌ها',
    'wishlist.remove': 'حذف از علاقه‌مندی‌ها',
    'account.loadingOrders': 'در حال بارگذاری سفارش‌ها…',
    'account.ordersError': 'بارگذاری سفارش‌ها ممکن نشد.',
    'account.noOrders': 'هنوز سفارشی ندارید.',
    'account.shopNow': 'همین حالا خرید کنید',
    'account.items': 'قلم',
    'account.backOrders': '→ بازگشت به سفارش‌ها',
    'account.orderTitle': 'سفارش #{id}',
    'account.savedAddresses': 'آدرس‌های ذخیره‌شده',
    'account.loadingAddresses': 'در حال بارگذاری آدرس‌ها…',
    'account.noAddresses': 'هنوز آدرس ذخیره‌شده‌ای نیست.',
    'account.default': 'پیش‌فرض',
    'account.edit': 'ویرایش',
    'account.delete': 'حذف',
    'account.addAddress': 'افزودن آدرس',
    'account.editAddress': 'ویرایش آدرس',
    'account.label': 'برچسب',
    'account.setDefault': 'تنظیم به‌عنوان آدرس پیش‌فرض',
    'account.saveAddress': 'ذخیره آدرس',
    'account.saving': 'در حال ذخیره…',
    'account.cancel': 'انصراف',
    'account.addressSaveFailed': 'ذخیره آدرس ممکن نشد.',
    'account.addressDeleteFailed': 'حذف آدرس ممکن نشد.',
    'account.addressesError': 'بارگذاری آدرس‌ها ممکن نشد.',
    'order.loading': 'در حال بارگذاری سفارش…',
    'order.notFound': 'سفارش پیدا نشد.',
    'order.back': 'بازگشت به فروشگاه',
    'order.thanks': 'متشکریم',
    'order.confirmed': 'سفارش #{id} تأیید شد',
    'order.pendingTitle': 'سفارش #{id} دریافت شد',
    'order.receipt':
      'رسید به {email} ایمیل شد. سفارش ولورای شما تأیید شده و در حال آماده‌سازی است.',
    'order.receiptPending':
      'سفارش شما دریافت شد و یادداشتی به {email} ارسال شد. پس از بررسی استودیو، وضعیت به تأیید شده تغییر می‌کند.',
    'order.status': 'وضعیت',
    'order.placed': 'زمان ثبت',
    'order.shipTo': 'ارسال به',
    'order.shipping': 'ارسال',
    'order.free': 'رایگان',
    'order.total': 'جمع کل',
    'order.continue': 'ادامه خرید',
    'order.status.Confirmed': 'تأیید شده',
    'order.status.Pending': 'در انتظار',
    'footer.tagline': 'لوکس آرام برای آیین‌های روزانه پوست.',
    'footer.shopAll': 'همه محصولات',
    'footer.skincare': 'مراقبت پوست',
    'footer.makeup': 'آرایش',
    'footer.legal': '© {year} زیبایی ولورا',
    'skin.All': 'همه',
    'skin.Dry': 'خشک',
    'skin.Normal': 'معمولی',
    'skin.Sensitive': 'حساس'
  }
};
