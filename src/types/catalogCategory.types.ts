export interface CatalogWordSummary {
  name: string;
  slug: string;
  description?: string;
}

export interface CatalogCategorySummary {
  slug: string;
  name: string;
  description?: string;
  sampleWords: CatalogWordSummary[];
}

export interface CatalogCategoryDetail {
  name: string;
  slug: string;
  description?: string;
  availableLetters: string[];
}
