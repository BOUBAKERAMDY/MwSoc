
package com.letsgo.ws.generated;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlRootElement;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Clase Java para anonymous complex type.
 * 
 * <p>El siguiente fragmento de esquema especifica el contenido que se espera que haya en esta clase.
 * 
 * <pre>
 * &lt;complexType&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="GetItineraryByCoordsResult" type="{http://schemas.datacontract.org/2004/07/LetsGoBiking.Shared}ItineraryDto" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "", propOrder = {
    "getItineraryByCoordsResult"
})
@XmlRootElement(name = "GetItineraryByCoordsResponse")
public class GetItineraryByCoordsResponse {

    @XmlElementRef(name = "GetItineraryByCoordsResult", namespace = "http://tempuri.org/", type = JAXBElement.class, required = false)
    protected JAXBElement<ItineraryDto> getItineraryByCoordsResult;

    /**
     * Obtiene el valor de la propiedad getItineraryByCoordsResult.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link ItineraryDto }{@code >}
     *     
     */
    public JAXBElement<ItineraryDto> getGetItineraryByCoordsResult() {
        return getItineraryByCoordsResult;
    }

    /**
     * Define el valor de la propiedad getItineraryByCoordsResult.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link ItineraryDto }{@code >}
     *     
     */
    public void setGetItineraryByCoordsResult(JAXBElement<ItineraryDto> value) {
        this.getItineraryByCoordsResult = value;
    }

}
