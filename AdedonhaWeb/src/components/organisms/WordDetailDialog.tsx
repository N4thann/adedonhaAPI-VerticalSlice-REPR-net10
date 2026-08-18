import { useEffect, useState } from 'react';
import { Alert, Box, Chip, CircularProgress, Dialog, DialogContent, DialogTitle, Stack, Typography } from '@mui/material';
import { catalogWordService } from '../../services/catalogWordService';
import type { CatalogWordDetail } from '../../types/catalogWord.types';

interface WordDetailDialogProps {
  slug?: string;
  onClose: () => void;
}

export const WordDetailDialog = ({ slug, onClose }: WordDetailDialogProps) => {
  const [detail, setDetail] = useState<CatalogWordDetail | undefined>(undefined);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!slug) {
      setDetail(undefined);
      return;
    }
    setIsLoading(true);
    setError(null);
    catalogWordService.getBySlug(slug)
      .then(setDetail)
      .catch(() => setError('Erro ao carregar detalhes da palavra.'))
      .finally(() => setIsLoading(false));
  }, [slug]);

  return (
    <Dialog open={!!slug} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>{detail?.name ?? 'Palavra'}</DialogTitle>
      <DialogContent>
        {isLoading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 2 }}>
            <CircularProgress size={24} />
          </Box>
        )}
        {error && <Alert severity="error">{error}</Alert>}
        {detail && (
          <Stack spacing={2}>
            <Typography variant="body1">{detail.description ?? 'Sem descrição.'}</Typography>
            {detail.categories.length > 0 && (
              <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
                {detail.categories.map((category) => (
                  <Chip key={category.slug} label={category.name} size="small" />
                ))}
              </Box>
            )}
          </Stack>
        )}
      </DialogContent>
    </Dialog>
  );
};
