import { useEffect, useState } from 'react';
import { Alert, Box, Card, CardActionArea, CircularProgress, Grid, Typography } from '@mui/material';
import { Category as CategoryIcon } from '@mui/icons-material';
import { catalogCategoryService } from '../services/catalogCategoryService';
import type { CatalogCategorySummary } from '../types/catalogCategory.types';
import { AlphabetDialog } from '../components/organisms/AlphabetDialog';
import { CategoryWordCountChart } from '../components/organisms/CategoryWordCountChart';

export const CategoriesMuralPage = () => {
  const [categories, setCategories] = useState<CatalogCategorySummary[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<CatalogCategorySummary | undefined>(undefined);

  useEffect(() => {
    catalogCategoryService.listMural()
      .then(setCategories)
      .catch(() => setError('Erro ao carregar categorias.'))
      .finally(() => setIsLoading(false));
  }, []);

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) return <Alert severity="error">{error}</Alert>;

  return (
    <Box>
      <Grid container spacing={2}>
        {categories.map((category) => (
          <Grid key={category.slug} size={{ xs: 12, sm: 6, lg: 3 }}>
            <Card sx={{ bgcolor: 'primary.main', color: 'primary.contrastText' }}>
              <CardActionArea
                onClick={() => setSelectedCategory(category)}
                sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 1, p: 3 }}
              >
                <CategoryIcon fontSize="large" />
                <Typography variant="h6">{category.name}</Typography>
              </CardActionArea>
            </Card>
          </Grid>
        ))}
      </Grid>

      <CategoryWordCountChart />

      <AlphabetDialog
        open={!!selectedCategory}
        categorySlug={selectedCategory?.slug}
        categoryName={selectedCategory?.name}
        onClose={() => setSelectedCategory(undefined)}
      />
    </Box>
  );
};
