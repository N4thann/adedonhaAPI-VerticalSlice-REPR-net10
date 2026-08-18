import { useEffect, useState } from 'react';
import { Alert, Box, Button, Chip, Snackbar, TextField, Typography } from '@mui/material';
import { Add, Delete, Edit, UploadFile } from '@mui/icons-material';
import { GenericTable } from '../../../components/organisms/GenericTable';
import { ConfirmDialog } from '../../../components/organisms/ConfirmDialog';
import { WordFormDialog } from '../../../components/organisms/WordFormDialog';
import { CsvImportDialog } from '../../../components/organisms/CsvImportDialog';
import { useWordStore } from '../../../store/word/useWordStore';
import { wordService } from '../../../services/wordService';
import type { WordDetail, WordListItem } from '../../../types/word.types';
import type { TableAction, TableColumn } from '../../../types/common.types';

export const WordsPage = () => {
  const { words, totalCount, page, pageSize, isLoading, error, fetchWords, createWord, updateWord, deleteWord, saveWordCategories } = useWordStore();
  const [search, setSearch] = useState('');
  const [formOpen, setFormOpen] = useState(false);
  const [editingWord, setEditingWord] = useState<WordDetail | undefined>(undefined);
  const [deletingWord, setDeletingWord] = useState<WordListItem | undefined>(undefined);
  const [isDeleting, setIsDeleting] = useState(false);
  const [importOpen, setImportOpen] = useState(false);
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null);

  useEffect(() => {
    fetchWords({ page: 1 });
  }, []);

  const handleSearch = () => fetchWords({ page: 1, search });

  const handleOpenCreate = () => { setEditingWord(undefined); setFormOpen(true); };

  const handleOpenEdit = async (word: WordListItem) => {
    const detail = await wordService.getById(word.id);
    setEditingWord(detail);
    setFormOpen(true);
  };

  const handleSubmit = async (name: string, description: string | undefined, categoryIds: string[]) => {
    if (editingWord) {
      await updateWord(editingWord.id, { name, description });
      await saveWordCategories(editingWord.id, categoryIds, editingWord.categoryIds);
      setSnackbarMessage('Palavra atualizada com sucesso.');
    } else {
      await createWord({ name, description, categoryIds });
      setSnackbarMessage('Palavra criada com sucesso.');
    }
    await fetchWords();
  };

  const handleConfirmDelete = async () => {
    if (!deletingWord) return;
    setIsDeleting(true);
    try {
      await deleteWord(deletingWord.id);
      setSnackbarMessage('Palavra excluída com sucesso.');
    } finally {
      setIsDeleting(false);
      setDeletingWord(undefined);
    }
  };

  const columns: TableColumn<WordListItem>[] = [
    { id: 'name', label: 'Nome' },
    { id: 'initialLetter', label: 'Letra inicial' },
    {
      id: 'categoryNames', label: 'Categorias',
      render: (w) => (
        <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
          {w.categoryNames.map((name) => <Chip key={name} label={name} size="small" />)}
        </Box>
      ),
    },
    { id: 'description', label: 'Descrição' },
  ];

  const actions: TableAction<WordListItem>[] = [
    { tooltip: 'Editar', icon: <Edit fontSize="small" />, onClick: handleOpenEdit },
    { tooltip: 'Excluir', icon: <Delete fontSize="small" />, color: 'error', onClick: setDeletingWord },
  ];

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">Palavras</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Button variant="outlined" startIcon={<UploadFile />} onClick={() => setImportOpen(true)}>Importar CSV</Button>
          <Button variant="contained" startIcon={<Add />} onClick={handleOpenCreate}>Nova palavra</Button>
        </Box>
      </Box>

      <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
        <TextField label="Buscar" value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && handleSearch()} size="small" />
        <Button onClick={handleSearch}>Buscar</Button>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <GenericTable
        data={words} columns={columns} actions={actions} getRowId={(w) => w.id}
        totalCount={totalCount} page={page - 1} pageSize={pageSize} isLoading={isLoading}
        onPageChange={(newPage) => fetchWords({ page: newPage + 1 })}
        onRowsPerPageChange={(newPageSize) => fetchWords({ page: 1, pageSize: newPageSize })}
      />

      <WordFormDialog open={formOpen} word={editingWord} onSubmit={handleSubmit} onClose={() => setFormOpen(false)} />

      <ConfirmDialog
        open={!!deletingWord}
        title="Excluir palavra"
        message={`Tem certeza que deseja excluir "${deletingWord?.name}"?`}
        onConfirm={handleConfirmDelete}
        onCancel={() => setDeletingWord(undefined)}
        isLoading={isDeleting}
      />

      <CsvImportDialog open={importOpen} onClose={() => setImportOpen(false)} onImported={() => fetchWords()} />

      <Snackbar open={!!snackbarMessage} autoHideDuration={4000} onClose={() => setSnackbarMessage(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}>
        <Alert severity="success" variant="filled" onClose={() => setSnackbarMessage(null)}>{snackbarMessage}</Alert>
      </Snackbar>
    </Box>
  );
};
