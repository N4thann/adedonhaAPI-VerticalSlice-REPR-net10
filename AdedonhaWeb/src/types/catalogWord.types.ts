export interface CatalogWordCategory {
  slug: string;
  name: string;
}

export interface CatalogWordDetail {
  name: string;
  description?: string;
  categories: CatalogWordCategory[];
}

export interface CatalogWordListItem {
  name: string;
  slug: string;
  description?: string;
}

export interface CatalogWordsPage {
  items: CatalogWordListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
}
