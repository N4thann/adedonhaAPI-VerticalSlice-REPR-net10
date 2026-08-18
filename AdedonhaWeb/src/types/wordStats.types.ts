export interface WordCategoryCount {
  name: string;
  slug: string;
  categoryCount: number;
}

export interface WordStats {
  totalWords: number;
  wordsInMultipleCategories: WordCategoryCount[];
}
