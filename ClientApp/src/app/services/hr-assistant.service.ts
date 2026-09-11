import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

export interface HrAssistantResponse {
  answer: string;
}

@Injectable({ providedIn: 'root' })
export class HrAssistantService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5118/api/hr/ask';

  askQuestion(question: string): Observable<HrAssistantResponse> {
    return this.http.post<HrAssistantResponse>(this.apiUrl, JSON.stringify(question), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
