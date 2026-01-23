declare namespace RetailPortal.Model.Db.Entities.Common.Enum {
	export enum ProductCategory {
		None = 0,
		Electronics = 1,
		Clothing = 2,
		HomeAppliances = 3,
		Books = 4,
		Sports = 5,
		Toys = 6,
		Beauty = 7,
		Automotive = 8,
		Grocery = 9,
		Health = 10,
		Furniture = 11,
		Jewelry = 12,
		Music = 13,
		OfficeSupplies = 14,
		PetSupplies = 15,
		Tools = 16,
		VideoGames = 17,
		Outdoor = 18,
		BabyProducts = 19,
		ArtSupplies = 20
	}
}
declare namespace RetailPortal.Model.DTOs.Auth {
	export interface IAuthResponse {
		email: string;
		firstName: string;
		id: number;
		lastName: string;
		token: string;
	}
	export interface ILoginRequest {
		anonymousId: string | null;
		email: string | null;
		lastLoginAt: Date | null;
		latitude: number | null;
		longitude: number | null;
		password: string;
		requestTime: Date | null;
		timezoneOffsetMinutes: number | null;
		userId: number | null;
	}
	export interface ILoginRequestT {
		anonymousId: string | null;
		email: string | null;
		lastLoginAt: Date | null;
		password: string;
	}
	export interface IRegisterRequest {
		email: string;
		firstName: string;
		lastName: string;
		password: string;
	}
	export interface ITokenExchangeRequest {
		email: string;
		name: string;
		tokenProvider: string;
	}
}
declare namespace RetailPortal.Model.DTOs.Common {
	export interface IODataResponse<T> {
		count: string | null;
		nextPage: string | null;
		value: T | null;
	}
}
declare namespace RetailPortal.Model.DTOs.Product {
	export interface ICreateProductRequest {
		description: string;
		name: string;
		price: RetailPortal.Model.DTOs.Product.IPriceRequest;
		quantity: number;
	}
	export interface IPrice {
		currency: string;
		value: number;
	}
	export interface IPriceRequest {
		currency: string;
		value: number;
	}
	export interface IProductResponse {
		category: RetailPortal.Model.Db.Entities.Common.Enum.ProductCategory;
		description: string;
		imageUrl: string | null;
		name: string;
		price: RetailPortal.Model.DTOs.Product.IPrice;
		productId: number;
		quantity: number;
		userId: number | null;
	}
}