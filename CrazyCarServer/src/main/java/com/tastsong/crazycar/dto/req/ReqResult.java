package com.tastsong.crazycar.dto.req;


import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.ToString;

import javax.validation.constraints.Min;
import javax.validation.constraints.NotNull;

@NoArgsConstructor
@AllArgsConstructor
@ToString
@Data

public class ReqResult {
    @NotNull
    @Min(0)
    private int cid;
    private int complete_time;
}