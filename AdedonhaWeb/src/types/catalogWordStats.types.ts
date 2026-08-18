export interface CatalogWordCategoryCount {
  name: string;
  slug: string;
  categoryCount: number;
}

export interface CatalogWordStats {
  totalWords: number;
  wordsInMultipleCategories: CatalogWordCategoryCount[];
}
