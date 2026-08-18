import { useEffect, useState, type FormEvent } from 'react';
import { Box, Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from '@mui/material';
import type { Category, CategoryCreatePayload } from '../../types/category.types';

interface CategoryFormDialogProps {
  open: boolean;
  category?: Category;
  onSubmit: (payload: CategoryCreatePayload) => Promise<void>;
  onClose: () => void;
}

export const CategoryFormDialog = ({ open, category, onSubmit, onClose }: CategoryFormDialogProps) => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    if (open) {
      setName(category?.name ?? '');
      setDescription(category?.description ?? '');
    }
  }, [open, category]);

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setIsSaving(true);
    try {
      await onSubmit({ name, description: description || undefined });
      onClose();
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{category ? 'Editar categoria' : 'Nova categoria'}</DialogTitle>
      <Box component="form" onSubmit={handleSubmit}>
        <DialogContent>
          <Stack spacing={2}>
            <TextField label="Nome" value={name} onChange={(e) => setName(e.target.value)} required fullWidth />
            <TextField label="Descrição" value={description} onChange={(e) => setDescription(e.target.value)} fullWidth multiline rows={3} />
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
