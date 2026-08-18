import type { ReactNode } from 'react';

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface TableColumn<T> {
  id: keyof T | string;
  label: string;
  width?: string;
  render?: (item: T) => ReactNode;
}

export interface TableAction<T> {
  tooltip: string | ((item: T) => string);
  icon: ReactNode | ((item: T) => ReactNode);
  onClick: (item: T) => void;
  color?: 'inherit' | 'primary' | 'secondary' | 'success' | 'error' | 'info' | 'warning';
  disabled?: (item: T) => boolean;
}
