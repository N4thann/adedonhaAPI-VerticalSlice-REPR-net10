import { useEffect, useState, type FormEvent } from 'react';
import { Alert, Autocomplete, Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from '@mui/material';
import { categoryService } from '../../services/categoryService';
import type { Category } from '../../types/category.types';
import type { WordDetail } from '../../types/word.types';

const MAX_CATEGORY_OPTIONS_PAGE_SIZE = 100;

interface WordFormDialogProps {
  open: boolean;
  word?: WordDetail;
  onSubmit: (name: string, description: string | undefined, categoryIds: string[]) => Promise<void>;
  onClose: () => void;
}

export const WordFormDialog = ({ open, word, onSubmit, onClose }: WordFormDialogProps) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [categoryOptions, setCategoryOptions] = useState<Category[]>([]);
  const [categoryOptionsError, setCategoryOptionsError] = useState<string | null>(null);
  const [selectedCategories, setSelectedCategories] = useState<Category[]>([]);
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    if (!open) return;
    setCategoryOptionsError(null);
    categoryService.list(1, MAX_CATEGORY_OPTIONS_PAGE_SIZE)
      .then((result) => setCategoryOptions(result.items))
      .catch(() => setCategoryOptionsError('Erro ao carregar categorias.'));
  }, [open]);

  useEffect(() => {
    if (!open) return;
    setName(word?.name ?? '');
    setDescription(word?.description ?? '');
  }, [open, word]);

  useEffect(() => {
    if (categoryOptions.length === 0) return;
    setSelectedCategories(categoryOptions.filter((c) => word?.categoryIds.includes(c.id)));
  }, [categoryOptions, word]);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setIsSaving(true);
    try {
      await onSubmit(name, description || undefined, selectedCategories.map((c) => c.id));
      onClose();
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{word ? 'Editar palavra' : 'Nova palavra'}</DialogTitle>
      <Box component="form" onSubmit={handleSubmit}>
        <DialogContent>
          <Stack spacing={2}>
            <TextField label="Nome" value={name} onChange={(e) => setName(e.target.value)} required fullWidth />
            <TextField label="Descrição" value={description} onChange={(e) => setDescription(e.target.value)} fullWidth multiline rows={2} />
            {categoryOptionsError && <Alert severity="error">{categoryOptionsError}</Alert>}
            <Autocomplete
              multiple
              options={categoryOptions}
              value={selectedCategories}
              getOptionLabel={(c) => c.name}
              isOptionEqualToValue={(a, b) => a.id === b.id}
              onChange={(_, value) => setSelectedCategories(value)}
              renderInput={(params) => <TextField {...params} label="Categorias" />}
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} disabled={isSaving}>Cancelar</Button>
          <Button type="submit" variant="contained" disabled={isSaving}>Salvar</Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
};
