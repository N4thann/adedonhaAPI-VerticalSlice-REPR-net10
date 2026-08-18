import { useEffect, useState } from 'react';
import { Box, CircularProgress, Paper, Typography } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import { PieChart } from '@mui/x-charts/PieChart';
import { catalogWordService } from '../../services/catalogWordService';
import { DONUT_CHART_COLORS } from '../../theme';
import type { CatalogWordStats } from '../../types/catalogWordStats.types';

export const WordsInMultipleCategoriesChart = () => {
  const theme = useTheme();
  const [stats, setStats] = useState<CatalogWordStats | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [hasError, setHasError] = useState(false);

  useEffect(() => {
    catalogWordService.getWordStats()
      .then(setStats)
      .catch(() => setHasError(true))
      .finally(() => setIsLoading(false));
  }, []);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
        <CircularProgress size={24} />
      </Box>
    );
  }

  if (hasError || !stats) return null;

  return (
    <Paper sx={{ p: 3, mt: 4, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <Typography variant="h6" sx={{ mb: 1 }}>Palavras em mais de uma categoria</Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
        Total de {stats.totalWords} palavra(s) cadastrada(s).
      </Typography>
      {stats.wordsInMultipleCategories.length === 0 ? (
        <Typography variant="body2" color="text.secondary">Nenhuma palavra está em mais de uma categoria.</Typography>
      ) : (
        <PieChart
          series={[{
            data: stats.wordsInMultipleCategories.map((w) => ({ id: w.slug, value: w.categoryCount, label: w.name })),
            innerRadius: 50,
            outerRadius: 100,
          }]}
          colors={DONUT_CHART_COLORS[theme.palette.mode]}
          width={320}
          height={240}
        />
      )}
    </Paper>
  );
};
