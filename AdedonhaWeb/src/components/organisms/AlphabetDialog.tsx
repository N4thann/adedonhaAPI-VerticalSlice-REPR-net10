import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Alert, Box, CircularProgress, Dialog, DialogContent, DialogTitle, IconButton } from '@mui/material';
import { catalogCategoryService } from '../../services/catalogCategoryService';

const ALPHABET = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'.split('');

interface AlphabetDialogProps {
  open: boolean;
  categorySlug?: string;
  categoryName?: string;
  onClose: () => void;
}

export const AlphabetDialog = ({ open, categorySlug, categoryName, onClose }: AlphabetDialogProps) => {
  const navigate = useNavigate();
  const [availableLetters, setAvailableLetters] = useState<string[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open || !categorySlug) return;
    setIsLoading(true);
    setError(null);
    catalogCategoryService.getBySlug(categorySlug)
      .then((detail) => setAvailableLetters(detail.availableLetters))
      .catch(() => setError('Erro ao carregar letras disponíveis.'))
      .finally(() => setIsLoading(false));
  }, [open, categorySlug]);

  const handleSelectLetter = (letter: string) => {
    navigate(`/categorias/${categorySlug}/${letter}`);
    onClose();
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>{categoryName ?? 'Escolha a letra'}</DialogTitle>
      <DialogContent>
        {isLoading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 2 }}>
            <CircularProgress size={24} />
          </Box>
        )}
        {error && <Alert severity="error">{error}</Alert>}
        {!isLoading && !error && (
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, justifyContent: 'center' }}>
            {ALPHABET.map((letter) => (
              <IconButton
                key={letter}
                onClick={() => handleSelectLetter(letter)}
                disabled={!availableLetters.includes(letter)}
                sx={{ border: '1px solid', borderColor: 'divider', width: 40, height: 40, borderRadius: '50%' }}
              >
                {letter}
              </IconButton>
            ))}
          </Box>
        )}
      </DialogContent>
    </Dialog>
  );
};
