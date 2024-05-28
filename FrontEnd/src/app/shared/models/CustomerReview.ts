
export class CustomerReview{
    reviewId: number = 0;
    customerId: number = 0;
    bodyDescription: string = '';
    rating: string = '';
    reviewDate: Date | null = null;
    modifiedDate: Date | null = null;

    constructor(){
        this.reviewDate = new Date(Date.now());
        this.modifiedDate = new Date(Date.now());
    }

}