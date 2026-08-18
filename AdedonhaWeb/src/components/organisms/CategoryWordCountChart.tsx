import { useEffect, useState } from 'react';
import { Box, CircularProgress, Paper, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { PieChart } from '@mui/x-charts/PieChart';
import { catalogCategoryService } from '../../services/catalogCategoryService';
import { catalogWordService } from '../../services/catalogWordService';
import { DONUT_CHART_COLORS } from '../../theme';
import type { CategoryWordCount } from '../../types/categoryWordCount.types';
import type { CatalogWordStats } from '../../types/catalogWordStats.types';

export const CategoryWordCountChart = () => {
  const theme = useTheme();
  const [counts, setCounts] = useState<CategoryWordCount[]>([]);
  const [wordStats, setWordStats] = useState<CatalogWordStats | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [hasError, setHasError] = useState(false);

  useEffect(() => {
    catalogCategoryService.getWordCounts()
      .then(setCounts)
      .catch(() => setHasError(true))
      .finally(() => setIsLoading(false));
    catalogWordService.getWordStats()
      .then(setWordStats)
      .catch(() => {});
  }, []);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress size={24} />
      </Box>
    );
  }

  if (hasError || counts.length === 0) return null;

  return (
    <Paper sx={{ p: 3, mt: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Typography variant="h6" sx={{ mb: 1 }}>Quantas palavras tem cada categoria?</Typography>
      {wordStats && (
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          Total de {wordStats.totalWords} palavra(s) cadastrada(s).
        </Typography>
      )}
      <PieChart
        series={[{
          data: counts.map((c) => ({ id: c.slug, value: c.wordCount, label: c.name })),
          innerRadius: 50,
          outerRadius: 100,
        }]}
        colors={DONUT_CHART_COLORS[theme.palette.mode]}
        width={320}
        height={240}
      />
    </Paper>
  );
};
