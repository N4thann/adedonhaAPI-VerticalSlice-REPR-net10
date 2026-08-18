export interface WordListItem {
  id: string;
  name: string;
  slug: string;
  initialLetter: string;
  description?: string;
  categoryNames: string[];
}

export interface WordDetail {
  id: string;
  name: string;
  slug: string;
  initialLetter: string;
  description?: string;
  categoryIds: string[];
}

export interface WordCreatePayload {
  name: string;
  description?: string;
  categoryIds?: string[];
}

export interface WordUpdatePayload {
  name: string;
  description?: string;
}

export interface BulkUploadRowError {
  line: number;
  reason: string;
}

export interface BulkUploadResult {
  totalRows: number;
  categoriesCreated: number;
  wordsCreated: number;
  associationsCreated: number;
  rowsSkipped: number;
  errors: BulkUploadRowError[];
}
