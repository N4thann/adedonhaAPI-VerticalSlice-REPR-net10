import { useEffect, useRef, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useTheme } from '@mui/material/styles';
import { Alert, Box, Button, Card, CardActionArea, CircularProgress, Grid, Typography, useMediaQuery } from '@mui/material';
import { ArrowBack, Info } from '@mui/icons-material';
import { useCategoryWordsStore } from '../store/catalogWords/useCategoryWordsStore';
import { catalogCategoryService } from '../services/catalogCategoryService';
import { WordDetailDialog } from '../components/organisms/WordDetailDialog';

export const CategoryWordsPage = () => {
  const { categorySlug = '', letra = '' } = useParams();
  const navigate = useNavigate();
  const theme = useTheme();
  const isSmUp = useMediaQuery(theme.breakpoints.up('sm'));
  const isLgUp = useMediaQuery(theme.breakpoints.up('lg'));
  const { words, isLoading, hasMore, error, initialize, loadNextPage } = useCategoryWordsStore();
  const [categoryName, setCategoryName] = useState(categorySlug);
  const [selectedWordSlug, setSelectedWordSlug] = useState<string | undefined>(undefined);
  const sentinelRef = useRef<HTMLDivElement | null>(null);

  const columns = isLgUp ? 4 : isSmUp ? 2 : 1;
  const pageSize = columns * 10;

  useEffect(() => {
    catalogCategoryService.getBySlug(categorySlug)
      .then((detail) => setCategoryName(detail.name))
      .catch(() => setCategoryName(categorySlug));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [categorySlug]);

  useEffect(() => {
    initialize(categorySlug, letra, pageSize);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [categorySlug, letra]);

  useEffect(() => {
    const sentinel = sentinelRef.current;
    if (!sentinel) return;

    const observer = new IntersectionObserver((entries) => {
      if (entries[0].isIntersecting) {
        loadNextPage(categorySlug, letra);
      }
    });
    observer.observe(sentinel);
    return () => observer.disconnect();
  }, [categorySlug, letra, loadNextPage]);

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <Button startIcon={<ArrowBack />} onClick={() => navigate('/')}>Voltar</Button>
        <Typography variant="h5">{categoryName} — {letra.toUpperCase()}</Typography>
      </Box>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      {words.length === 0 && !isLoading && !error && (
        <Alert severity="info">Nenhuma palavra encontrada.</Alert>
      )}

      <Grid container spacing={2}>
        {words.map((word) => (
          <Grid key={word.slug} size={{ xs: 12, sm: 6, lg: 3 }}>
            <Card sx={{ bgcolor: 'secondary.main', color: 'secondary.contrastText' }}>
              <CardActionArea
                onClick={() => setSelectedWordSlug(word.slug)}
                sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', p: 2 }}
              >
                <Typography>{word.name}</Typography>
                <Info />
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Box ref={sentinelRef} sx={{ display: 'flex', justifyContent: 'center', py: 2 }}>
        {isLoading && <CircularProgress size={24} />}
        {!hasMore && words.length > 0 && (
          <Typography variant="body2" color="text.secondary">Fim da lista.</Typography>
        )}
      </Box>

      <WordDetailDialog slug={selectedWordSlug} onClose={() => setSelectedWordSlug(undefined)} />
    </Box>
  );
};
