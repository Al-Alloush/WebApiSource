import { IExternaPage } from "./IExternaPage";
import { ILanguages } from "./ILanguages";

export interface ISharedData {
    headerWelcomMessage: string;
    footerWelcomMessage: string;
    email: string;
    phoneNumber: string;
    address: string;
    fax: string;
    subscribeTitle: string;
    subscribeSubTitle: string;
    companyLocation: string;
    languages: ILanguages[];
    externalPages: IExternaPage[];
    internalPages: IExternaPage[];
}
