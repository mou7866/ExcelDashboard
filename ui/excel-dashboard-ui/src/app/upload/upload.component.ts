import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss'],
})
export class UploadComponent {
  private readonly api = inject(ApiService);

  file: File | null = null;
  currency = 'USD';
  isUploading = signal(false);
  message = signal<string | null>(null);
  error = signal<string | null>(null);

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files[0];
    this.file = file ?? null;
    this.message.set(null);
    this.error.set(null);
  }

  upload(): void {
    if (!this.file) {
      this.error.set('Please select an .xlsx file.');
      return;
    }

    this.isUploading.set(true);
    this.error.set(null);
    this.message.set(null);

    this.api.upload(this.file, this.currency).subscribe({
      next: () => {
        this.isUploading.set(false);
        this.message.set('File uploaded successfully.');
      },
      error: () => {
        this.isUploading.set(false);
        this.error.set('Upload failed. Check the server log.');
      },
    });
  }
}
