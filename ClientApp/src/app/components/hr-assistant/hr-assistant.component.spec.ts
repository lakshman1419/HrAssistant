import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HrAssistantComponent } from './hr-assistant.component';

describe('HrAssistantComponent', () => {
  let fixture: ComponentFixture<HrAssistantComponent>;
  let component: HrAssistantComponent;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HrAssistantComponent, HttpClientTestingModule]
    }).compileComponents();

    fixture = TestBed.createComponent(HrAssistantComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => httpMock.verify());

  it('adds a user question and displays the API answer', () => {
    component.questionControl.setValue('What is the leave policy?');
    component.sendMessage();

    const request = httpMock.expectOne('http://localhost:5118/api/hr/ask');
    expect(request.request.body).toBe('"What is the leave policy?"');
    request.flush({ answer: 'Please review the leave policy.' });

    expect(component.messages.at(-1)?.content).toBe('Please review the leave policy.');
    expect(component.isLoading).toBeFalse();
  });

  it('does not submit an empty question', () => {
    component.questionControl.setValue('   ');
    component.sendMessage();

    httpMock.expectNone('http://localhost:5118/api/hr/ask');
    expect(component.messages).toHaveSize(1);
  });
});
