import { Box, IconButton, TextField, Button } from '@mui/material';
import Add from '@mui/icons-material/Add';
import Delete from '@mui/icons-material/Delete';

interface StringListEditorProps {
  itemLabel: string;
  value: string[];
  onChange: (value: string[]) => void;
}

export const StringListEditor = ({ itemLabel, value, onChange }: StringListEditorProps) => {
  const handleAdd = () => {
    onChange([...value, '']);
  };

  const handleRemove = (index: number) => {
    onChange(value.filter((_, i) => i !== index));
  };

  const handleChange = (index: number, newValue: string) => {
    onChange(value.map((item, i) => (i === index ? newValue : item)));
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      {value.map((item, index) => (
        <Box key={index} sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
          <TextField
            label={`${itemLabel} ${index + 1}`}
            value={item}
            onChange={(e) => handleChange(index, e.target.value)}
            fullWidth
          />
          <IconButton aria-label={`Remover ${itemLabel.toLowerCase()} ${index + 1}`} onClick={() => handleRemove(index)}>
            <Delete />
          </IconButton>
        </Box>
      ))}
      <Button startIcon={<Add />} onClick={handleAdd} sx={{ alignSelf: 'flex-start' }}>
        Adicionar {itemLabel.toLowerCase()}
      </Button>
    </Box>
  );
};
