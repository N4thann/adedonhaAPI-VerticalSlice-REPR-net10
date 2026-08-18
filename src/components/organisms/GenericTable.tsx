import type { ReactNode } from 'react';
import {
  Box, CircularProgress, IconButton, Paper, Table, TableBody, TableCell,
  TableContainer, TableHead, TablePagination, TableRow, Tooltip, Typography,
} from '@mui/material';
import type { TableAction, TableColumn } from '../../types/common.types';

interface GenericTableProps<T> {
  data: T[];
  columns: TableColumn<T>[];
  actions?: TableAction<T>[];
  getRowId: (item: T) => string;
  totalCount: number;
  page: number;
  pageSize: number;
  onPageChange: (newPage: number) => void;
  onRowsPerPageChange: (newPageSize: number) => void;
  isLoading?: boolean;
}

export function GenericTable<T>({
  data, columns, actions, getRowId, totalCount, page, pageSize, onPageChange, onRowsPerPageChange, isLoading,
}: GenericTableProps<T>) {
  const columnCount = columns.length + (actions && actions.length > 0 ? 1 : 0);

  const renderCell = (column: TableColumn<T>, item: T): ReactNode =>
    column.render ? column.render(item) : String(item[column.id as keyof T] ?? '');

  return (
    <Paper>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              {columns.map((column) => (
                <TableCell key={String(column.id)}>{column.label}</TableCell>
              ))}
              {actions && actions.length > 0 && <TableCell align="right">Ações</TableCell>}
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow>
                <TableCell colSpan={columnCount} align="center">
                  <CircularProgress size={24} sx={{ my: 2 }} />
                </TableCell>
              </TableRow>
            ) : data.length === 0 ? (
              <TableRow>
                <TableCell colSpan={columnCount} align="center">
                  <Typography variant="body2" color="text.secondary" sx={{ my: 2 }}>
                    Nenhum registro encontrado.
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              data.map((item) => (
                <TableRow key={getRowId(item)}>
                  {columns.map((column) => (
                    <TableCell key={String(column.id)}>{renderCell(column, item)}</TableCell>
                  ))}
                  {actions && actions.length > 0 && (
                    <TableCell align="right">
                      <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 0.5 }}>
                        {actions.map((action, index) => {
                          const isDisabled = action.disabled ? action.disabled(item) : false;
                          const tooltip = typeof action.tooltip === 'function' ? action.tooltip(item) : action.tooltip;
                          const icon = typeof action.icon === 'function' ? action.icon(item) : action.icon;
                          return (
                            <Tooltip key={index} title={tooltip}>
                              <span>
                                <IconButton size="small" color={action.color ?? 'default'} disabled={isDisabled} onClick={() => action.onClick(item)}>
                                  {icon}
                                </IconButton>
                              </span>
                            </Tooltip>
                          );
                        })}
                      </Box>
                    </TableCell>
                  )}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={totalCount}
        page={page}
        rowsPerPage={pageSize}
        rowsPerPageOptions={[5, 10, 25]}
        labelRowsPerPage="Linhas por página:"
        onPageChange={(_, newPage) => onPageChange(newPage)}
        onRowsPerPageChange={(e) => onRowsPerPageChange(Number(e.target.value))}
      />
    </Paper>
  );
}
