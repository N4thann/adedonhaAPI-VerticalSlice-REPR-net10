import { useEffect, useState } from 'react';
import { Alert, Box, Button, Snackbar, TextField, Typography } from '@mui/material';
import { Add, Delete, Edit } from '@mui/icons-material';
import { GenericTable } from '../../../components/organisms/GenericTable';
import { ConfirmDialog } from '../../../components/organisms/ConfirmDialog';
import { CategoryFormDialog } from '../../../components/organisms/CategoryFormDialog';
import { useCategoryStore } from '../../../store/category/useCategoryStore';
import type { Category } from '../../../types/category.types';
import type { TableAction, TableColumn } from '../../../types/common.types';
import { extractApiErrorMessage } from '../../../utils/apiError';

export const CategoriesPage = () => {
  const { categories, totalCount, page, pageSize, isLoading, error, fetchCategories, createCategory, updateCategory, deleteCategory } = useCategoryStore();
  const [search, setSearch] = useState('');
  const [formOpen, setFormOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | undefined>(undefined);
  const [deletingCategory, setDeletingCategory] = useState<Category | undefined>(undefined);
  const [isDeleting, setIsDeleting] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null);
  const [deleteErrorMessage, setDeleteErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    fetchCategories({ page: 1 });
  }, []);

  const handleSearch = () => fetchCategories({ page: 1, search });

  const handleOpenCreate = () => { setEditingCategory(undefined); setFormOpen(true); };
  const handleOpenEdit = (category: Category) => { setEditingCategory(category); setFormOpen(true); };

  const handleSubmit = async (payload: { name: string; description?: string }) => {
    if (editingCategory) {
      await updateCategory(editingCategory.id, payload);
      setSnackbarMessage('Categoria atualizada com sucesso.');
    } else {
      await createCategory(payload);
      setSnackbarMessage('Categoria criada com sucesso.');
    }
  };

  const handleConfirmDelete = async () => {
    if (!deletingCategory) return;
    setIsDeleting(true);
    try {
      await deleteCategory(deletingCategory.id);
      setSnackbarMessage('Categoria excluída com sucesso.');
    } catch (err) {
      setDeleteErrorMessage(extractApiErrorMessage(err, 'Erro ao excluir categoria.'));
    } finally {
      setIsDeleting(false);
      setDeletingCategory(undefined);
    }
  };

  const columns: TableColumn<Category>[] = [
    { id: 'name', label: 'Nome' },
    { id: 'slug', label: 'Slug' },
    { id: 'description', label: 'Descrição', width: '40%' },
  ];

  const actions: TableAction<Category>[] = [
    { tooltip: 'Editar', icon: <Edit fontSize="small" />, onClick: handleOpenEdit },
    { tooltip: 'Excluir', icon: <Delete fontSize="small" />, color: 'error', onClick: setDeletingCategory },
  ];

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Categorias</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={handleOpenCreate}>Nova categoria</Button>
      </Box>

      <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
        <TextField label="Buscar" value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && handleSearch()} size="small" />
        <Button onClick={handleSearch}>Buscar</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <GenericTable
        data={categories} columns={columns} actions={actions} getRowId={(c) => c.id}
        totalCount={totalCount} page={page - 1} pageSize={pageSize} isLoading={isLoading}
        onPageChange={(newPage) => fetchCategories({ page: newPage + 1 })}
        onRowsPerPageChange={(newPageSize) => fetchCategories({ page: 1, pageSize: newPageSize })}
      />

      <CategoryFormDialog open={formOpen} category={editingCategory} onSubmit={handleSubmit} onClose={() => setFormOpen(false)} />

      <ConfirmDialog
        open={!!deletingCategory}
        title="Excluir categoria"
        message={`Tem certeza que deseja excluir "${deletingCategory?.name}"?`}
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeletingCategory(undefined)}
        isLoading={isDeleting}
      />

      <Snackbar open={!!snackbarMessage} autoHideDuration={4000} onClose={() => setSnackbarMessage(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity="success" variant="filled" onClose={() => setSnackbarMessage(null)}>{snackbarMessage}</Alert>
      </Snackbar>

      <Snackbar open={!!deleteErrorMessage} autoHideDuration={6000} onClose={() => setDeleteErrorMessage(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity="error" variant="filled" onClose={() => setDeleteErrorMessage(null)}>{deleteErrorMessage}</Alert>
      </Snackbar>
    </Box>
  );
};
